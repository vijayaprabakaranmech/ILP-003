import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';

import { RequestService } from "./shared/request.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  status : boolean = false;
  disp : boolean = false;
  constructor(private requestService : RequestService) { }
  onSubmit(form : NgForm){
   this.status = true;
    this.requestService.postRequest(form.value)
    .subscribe ( data => {
      this.requestService.getIP();
      alert("Success....Please note the credentials....");
      this.status = false;
      this.disp = true;
      setTimeout(function(){
        window.location.reload();
      },60000);
  }) 
  }
}
