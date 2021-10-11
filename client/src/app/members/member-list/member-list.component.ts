import { Component, OnInit } from '@angular/core';

 import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/Pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Member[] | null;
  pagination: Pagination;
  userParams: UserParams;
  user: User|null;
  genderList=[{value:'male',display:'Males'},{value:'female',display:'Females'}]

  constructor(private memberSrevices: MembersService) {
    this.userParams=this.memberSrevices.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  
  loadMembers() {
    this.memberSrevices.setUserParams(this.userParams);
    this.memberSrevices.getmembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  resetFilters(){
     this.userParams = this.memberSrevices.resetUserParams();
    this.loadMembers();
  }
 
  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.memberSrevices.setUserParams(this.userParams);
    this.loadMembers();
  }


}
