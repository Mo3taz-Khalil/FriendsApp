import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { Pagination } from 'src/app/_models/Pagination';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  //static makes it dynamic to be able to react to changes in our component
  @ViewChild('memberTabs',{static:true}) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] | null = [];
  pageNumber = 1;
  pageSize = 20;
  pagination: Pagination;


  constructor(private memberService: MembersService, private route: ActivatedRoute,
    private messageServices: MessageService) { }

  ngOnInit(): void {

    this.route.data.subscribe(data=>{
      this.member=data.member;
    });

    this.route.queryParams.subscribe(params=>{
      params.tab ? this.selectTab(3) : this.selectTab(0);
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.getImages();

  }

  getImages(): NgxGalleryImage[] {
    const imgUrls = [];
    for (const photo of this.member.photos) {
      imgUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imgUrls;
  }
//استخدمت الراوتر مكانها بس لسه السؤال موجود 
  // loadMember() {
  //   this.memberService.getmember(this.route.snapshot.paramMap.get('username')).subscribe(member => {
  //     this.member = member;
  //     //ليه متنفعش غير هنا مش بعد الكود ده ؟؟؟
  //   })
  //   //هنا يعني متنفعش ليه 
  // }

  loadMessages() {
    this.messageServices.getMessageThread(this.member.username, this.pageNumber, this.pageSize).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
    })
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(date: TabDirective) {
    this.activeTab = date;
    if (this.activeTab.heading == 'Messages' && this.messages?.length === 0) {
      this.loadMessages();
    }
  }


}
