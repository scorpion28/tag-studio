import { Injectable } from "@angular/core";
import { JwtTokenData } from "../models/jwt-token-data.model";
import { UserInfo } from '../models/user-info-brief.model';

@Injectable({ providedIn: "root" })
export class StorageService {
  private readonly STORAGE_KEY = "jwt";

  getTokenData(): JwtTokenData | null {
    const tokenData = localStorage.getItem(this.STORAGE_KEY);
    if(!tokenData) return null;

    try {
      return JSON.parse(tokenData);
    } catch (e) {
      console.error(e);
      return null;
    }
  }

  getAccessToken(): string | null {
    return this.getTokenData()?.accessToken || null;
  }

  getRefreshToken(): string | null {
    return this.getTokenData()?.refreshToken || null;
  }

  saveTokenData(jwtData: JwtTokenData): void {
    localStorage[this.STORAGE_KEY] = JSON.stringify(jwtData);
  }

  removeTokenData(): void {
    localStorage.removeItem(this.STORAGE_KEY);
  }

  getUserInfo(): UserInfo | null {
    const userInfo = localStorage.getItem('user');
    if (!userInfo) return null;

    try {
      return JSON.parse(userInfo);
    } catch (e) {
      console.error(e);
      return null;
    }
  }

  saveUserInfo(userInfo: UserInfo) {
    localStorage['user'] = JSON.stringify(userInfo);
  }

  removeUserInfo() {
    localStorage.removeItem('user');
  }
}
