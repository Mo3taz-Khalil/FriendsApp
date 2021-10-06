import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationParams } from './PaginationHelpers';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationParams(pageNumber,pageSize);
    params= params.append('Container',container);

    return getPaginatedResult<Message[]>(this.baseUrl+'messages',params,this.http);
  }

  getMessageThread(username:string,pageNumber: number, pageSize: number)
  {
    let params = getPaginationParams(pageNumber,pageSize);

    return getPaginatedResult<Message[]>(this.baseUrl+'messages/thread/' + username,params,this.http)
  }

  sendMessage(username:string,content:string){
   return this.http.post<Message>(this.baseUrl+'messages',{RecipientUsername : username,content})
  }

  deleteMessage(id:number){
   return this.http.delete(this.baseUrl+'messages/'+id);
  }

}
