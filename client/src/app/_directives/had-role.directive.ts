import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHadRole]'  //*appHadRole='["admin"]'
})
export class HadRoleDirective implements OnInit { //onInit to acces on intialization

  @Input() appHadRole: string[]; // Directive ليه لازم نفس اسم ال 
  user: User | null;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }
  ngOnInit(): void {
    //clear view if no roles
    if (!this.user?.roles || this.user == null)//not authenticated
    {
      this.viewContainerRef.clear();
      return;
    }
    if (this.user?.roles.some(r => this.appHadRole?.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }

  }

}
