import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {

//get data before the html load 
//we can not acces navigation extras here
  constructor(private memberServices:MembersService) { }


  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberServices.getmember(route.paramMap.get('username'));
  }
}
