import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'The Friends App';
  
  constructor( private accountService: AccountService) { }

  ngOnInit() {    
    this.setCurrentUser();
  }

  setCurrentUser() {
    
    if(localStorage.getItem('user')){

      const user: User = JSON.parse(localStorage.getItem('user')!);
      this.accountService.setCurrentUser(user);
    }
    // if (user.username != null) {
    // }
    else {
      this.accountService.setCurrentUser(null);
    }
  }

  


}
