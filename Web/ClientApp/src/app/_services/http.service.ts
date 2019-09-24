import { HallOfFame } from './../_models/hallOfFame';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  constructor(private _httpClient: HttpClient) {}

  getHallOfFameStats() { 
    return this._httpClient.get<HallOfFame[]>('/api/halloffame');
  }
  getOnlineUsers() { 
    return this._httpClient.get<string[]>('/api/repository/users');
  }
}
