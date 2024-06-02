import {UserInfoBrief} from './user-info-brief.model';
import {JwtTokenData} from './jwt-token-data.model';

export interface LoginResponse {
  jwt: JwtTokenData;
  user: UserInfoBrief;
}
