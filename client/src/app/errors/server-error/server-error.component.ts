import { Component, isDevMode, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  divmode:boolean;
  error: any;
  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.error;
    if(isDevMode()){
      this.divmode=true
    }else this.divmode=false;
  }

  ngOnInit(): void {
  }

}
