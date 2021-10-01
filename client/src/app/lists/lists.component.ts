import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/Pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Partial<Member[]>|null
  predicate='likedByMeToUsers';
  pageNumber=1;
  pageSize=5;
  pagination:Pagination;
  constructor(private mamberService:MembersService) { }

  ngOnInit(): void {
    this.LoadLikes();
  }
  LoadLikes(){
    this.mamberService.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe(respons=>{
      this.members=respons.result;
      this.pagination=respons.pagination;
    })
  }

  pageChanged(event:any){
    this.pageNumber=event.page;
    this.LoadLikes();
  }
}
