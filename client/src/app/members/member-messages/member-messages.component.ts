import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { Pagination } from 'src/app/_models/Pagination';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @ViewChild('messageForm') messageForm:NgForm;
  @Input() username: string;
  @Input() messages: Message[] | null = [];
  pageNumber = 1;
  pageSize = 20;
  pagination: Pagination;
  messageContent:string;

  constructor(public messageServices: MessageService) { }

  ngOnInit(): void {
  }
  //هنا عاوز function من component اخدها في واحد تاني 
  loadMessages() {
    this.messageServices.getMessageThread(this.username, this.pageNumber, this.pageSize).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
    })
  }


  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMessages();
  }

  sendMessage(){
    this.messageServices.sendMessage(this.username,this.messageContent).then(()=>{
      this.messageForm.reset();
    })
  }
}
