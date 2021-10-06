import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
// import { url } from 'inspector';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/Pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationParams } from './PaginationHelpers';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  membersCash = new Map();
  userParams: UserParams;
  user: User | null;


  constructor(private http: HttpClient, private accountService: AccountService,private router:Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user!);
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams=new UserParams(this.user!);
    return this.userParams
  }

  getmembers(userParams: UserParams) {
    var response = this.membersCash.get(Object.values(userParams).join('-'));
    if (response) return of(response);

    let params = getPaginationParams(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params,this.http)
      .pipe(map(response => {
        this.membersCash.set(Object.values(userParams).join('-'), response);
        return response
      }))
  }

  getmember(username: string | null) {
    var member = [...this.membersCash.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }


  UpdateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  //#region photo
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  //#endregion

  //#region  likes
  addLike(username:string){
    // 
    
      let currentUrl = this.router.url;
      this.router.navigateByUrl('/', {skipLocationChange: true}).then(() => {
          this.router.navigate([currentUrl]);          
      });
     
   return this.http.post(this.baseUrl + "likes/"+username,{},{responseType: 'text'});
  }

  getLikes(predicate:string,pageNumber:number,pageSize:number){
    let params = getPaginationParams(pageNumber,pageSize);
    params=params.append('predicate',predicate);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl+'likes',params,this.http);
  }

  //#endregion


  


}




