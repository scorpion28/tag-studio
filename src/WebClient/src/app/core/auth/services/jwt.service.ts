import { Injectable } from "@angular/core";
import { JwtTokenData } from "../models/jwt-token-data.model";

@Injectable({ providedIn: "root" })
export class JwtService {
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

  saveToken(jwtData: JwtTokenData): void {
    localStorage[this.STORAGE_KEY] = JSON.stringify(jwtData);
  }

  destroyTokenData(): void {
    window.localStorage.removeItem(this.STORAGE_KEY);
  }

  isAccessTokenValid(): boolean {
    const tokenData = this.getTokenData();
    if (!tokenData) {
      return false;
    }
    return tokenData.expiresAtUtc > Date.now();
  }
}
