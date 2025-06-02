import { Injectable } from "@angular/core";
import { JwtTokenData } from "../models/jwt-token-data.model";

@Injectable({ providedIn: "root" })
export class JwtService {
  getTokenData(): JwtTokenData | null {
    const tokenData = localStorage.getItem("jwt");
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
    localStorage["jwt"] = JSON.stringify(jwtData);
  }

  destroyTokenData(): void {
    window.localStorage.removeItem("jwt");
  }

  isAccessTokenValid(): boolean {
    const tokenData = this.getTokenData();
    if (!tokenData) {
      return false;
    }
    return tokenData.expiresAtUtc > Date.now();
  }
}
