/*
 * Represents the data returned after a successful authentication
 * or token refresh operation, containing both the access token,
 * refresh token, and the expiration time of the access token.
 */
export interface JwtTokenData {
  accessToken: string;
  refreshToken: string;

  /**
   * The expiration time of the access token.
   * This is a UTC Unix timestamp represented as the number of milliseconds
   * that have elapsed since the Unix epoch (January 1, 1970, 00:00:00 UTC).
   */
  expiresAtUtc: number;
}
