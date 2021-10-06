import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messeges',
  templateUrl: './messeges.component.html',
  styleUrls: ['./messeges.component.css']
})
export class MessegesComponent implements OnInit {

  messages: Message[] | null=[];
  container = 'Unread';
  pageNumer = 1;
  pageSize = 5;
  pagination: Pagination;
  loadingFlag = false;

  constructor(private messagesServices: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    this.loadingFlag = true;
    this.messagesServices.getMessages(this.pageNumer, this.pageSize, this.container).subscribe(repsonse => {
      this.messages = repsonse.result;
      this.pagination = repsonse.pagination;
      this.loadingFlag = false;
    });
  }

  pageChanges(event: any) {
    this.pageNumer = event.page;
    this.loadMessages();
  }

  deleteMessage(id:number){
    this.messagesServices.deleteMessage(id).subscribe(()=>{
      this.messages?.splice(this.messages.findIndex(m=>m.id===id),1);
    })
  }


}
