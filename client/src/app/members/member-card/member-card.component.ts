import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PressenceService } from 'src/app/_services/pressence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;
  // onlineMembers:string[]=[];
  messag: string;

  constructor(private memberService: MembersService,
     private toastr: ToastrService,public pressence:PressenceService) { }

  addLike(member: Member) {
    this.memberService.addLike(member.username).subscribe((x) => {
      this.toastr.success(x + member.knownAs);
    })

  }




  ngOnInit(): void {
    
  }

}
