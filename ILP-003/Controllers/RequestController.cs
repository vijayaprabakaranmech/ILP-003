using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ILP_003.Models;

namespace ILP_003.Controllers
{
    public class RequestController : ApiController
    {
        private RequestContext db = new RequestContext();

        // GET: api/Request
        [ResponseType(typeof(Request))]
        public IHttpActionResult Getrequests()
        {
            var ip = db.requests.OrderByDescending(p => p.Id).FirstOrDefault().IP;
            return Ok(ip);
        }

        // GET: api/Request/5
        [ResponseType(typeof(Request))]
        public IHttpActionResult GetRequest(int id)
        {
            Request request = db.requests.Find(id);
            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }


        // POST: api/Request
        [ResponseType(typeof(Request))]
        public IHttpActionResult PostRequest(Request request)
        {
            request.RequestedDateTime = DateTime.Now;

            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal("c99526ff-46a7-4c14-a3dd-c007179c3f1f",
                "fuKHqKJOJfB4ZiU2AaNMGx3qyMzlsaviKZ3b2L73z1s=",
                "54c2e137-49f4-4774-afbe-172aae73f061",
                AzureEnvironment.AzureGlobalCloud);

            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(credentials)
                .WithSubscription("dab0b14c-927c-4d0a-99b6-2ae522a7f341");

            var groupName = request.UserName + "RG";
            var vmName = request.UserName + "VM";
            var location = Region.USWest;

            var resourceGroup = azure.ResourceGroups.Define(groupName)
                .WithRegion(location)
                .Create();

            if (request.VMType.Equals(".NET SDK"))
            {
                var availabilitySet = azure.AvailabilitySets.Define(request.UserName + "AVSet")
                    .WithRegion(location)
                    .WithExistingResourceGroup(groupName)
                    .WithSku(AvailabilitySetSkuTypes.Managed)
                    .Create();

                var publicIPAddress = azure.PublicIPAddresses.Define(request.UserName + "PublicIP")
                    .WithRegion(location)
                    .WithExistingResourceGroup(groupName)
                    .WithDynamicIP()
                    .Create();

                var network = azure.Networks.Define(request.UserName + "VNet")
                    .WithRegion(location)
                    .WithExistingResourceGroup(groupName)
                    .WithAddressSpace("10.0.0.0/16")
                    .WithSubnet("mySubnet", "10.0.0.0/24")
                    .Create();

                var networkInterface = azure.NetworkInterfaces.Define(request.UserName + "NIC")
                    .WithRegion(location)
                    .WithExistingResourceGroup(groupName)
                    .WithExistingPrimaryNetwork(network)
                    .WithSubnet("mySubnet")
                    .WithPrimaryPrivateIPAddressDynamic()
                    .WithExistingPrimaryPublicIPAddress(publicIPAddress)
                    .Create();

                azure.VirtualMachines.Define(vmName)
                    .WithRegion(location)
                    .WithExistingResourceGroup(groupName)
                    .WithExistingPrimaryNetworkInterface(networkInterface)
                    .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer", "2012-R2-Datacenter")
                    .WithAdminUsername("AzureUser")
                    .WithAdminPassword("Vijay@123")
                    .WithComputerName(vmName)
                    .WithExistingAvailabilitySet(availabilitySet)
                    .WithSize(VirtualMachineSizeTypes.StandardDS1)
                    .Create();

                var vm = azure.VirtualMachines.GetByResourceGroup(groupName, vmName);

                request.VMSize = vm.Size.ToString();
                request.IP = vm.GetPrimaryPublicIPAddress().IPAddress;
            }

            else
            {
                string storageAccountName = SdkContext.RandomResourceName("st", 10);

                Console.WriteLine("Creating storage account...");
                var storage = azure.StorageAccounts.Define(storageAccountName)
                    .WithRegion(Region.USWest)
                    .WithExistingResourceGroup(resourceGroup)
                    .Create();

                var storageKeys = storage.GetKeys();
                string storageConnectionString = "DefaultEndpointsProtocol=https;"
                    + "AccountName=" + storage.Name
                    + ";AccountKey=" + storageKeys[0].Value
                    + ";EndpointSuffix=core.windows.net";

                var account = CloudStorageAccount.Parse(storageConnectionString);
                var serviceClient = account.CreateCloudBlobClient();

                var container = serviceClient.GetContainerReference("templates");
                container.CreateIfNotExistsAsync().Wait();
                var containerPermissions = new BlobContainerPermissions()
                { PublicAccess = BlobContainerPublicAccessType.Container };
                container.SetPermissionsAsync(containerPermissions).Wait();

                var templateblob = container.GetBlockBlobReference("CreateVMTemplate.json");
                templateblob.UploadFromFileAsync("F:\\Vijay\\ILP-003\\ILP-003\\CreateVMTemplate.json");

                var paramblob = container.GetBlockBlobReference("Parameters.json");
                paramblob.UploadFromFileAsync("F:\\Vijay\\ILP-003\\ILP-003\\Parameters.json");

                var templatePath = "https://" + storageAccountName + ".blob.core.windows.net/templates/CreateVMTemplate.json";
                var paramPath = "https://" + storageAccountName + ".blob.core.windows.net/templates/Parameters.json";
                var deployment = azure.Deployments.Define("myDeployment")
                    .WithExistingResourceGroup(groupName)
                    .WithTemplateLink(templatePath, "1.0.0.0")
                    .WithParametersLink(paramPath, "1.0.0.0")
                    .WithMode(Microsoft.Azure.Management.ResourceManager.Fluent.Models.DeploymentMode.Incremental)
                    .Create();

                var vm = azure.VirtualMachines.GetByResourceGroup(groupName, "myVM");
                request.VMSize = vm.Size.ToString();
                request.IP = vm.GetPrimaryPublicIPAddress().IPAddress;
            }

            db.requests.Add(request);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = request.Id }, request);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RequestExists(int id)
        {
            return db.requests.Count(e => e.Id == id) > 0;
        }
    }
}