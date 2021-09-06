import { HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getmembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getmember(username: string|null) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
}


