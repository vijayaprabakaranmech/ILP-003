import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, RequestMethod, Jsonp, BaseRequestOptions } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';

@Injectable({
  providedIn: 'root'
})
export class RequestService {

  constructor(private http : Http) { }
    UserName : String;
    EmailID : String;
    Location : String;
    Purpose : String;
    Duration : number;
    VMType : String;
    IP : String;

  postRequest(request : Request){
    var body = JSON.stringify(request);
    var headerOptions = new Headers({'Content-Type':'application/json'});
    var requestOptions = new RequestOptions({method : RequestMethod.Post, headers : headerOptions});
    return  this.http.post('http://localhost:54527/api/Request', body, requestOptions).map(x => x.json());
  }

  getIP(){
    this.http.get('http://localhost:54527/api/Request')
    .map((data : Response) => {
      return data.json();
    }).toPromise().then(x => {
      this.IP = "IP : "+ x.toString();
    })
  }
}
