export interface AuthError {
  message: string;
  details?: ProblemDetails | ValidationError
  statusCode?: number;
}

export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;

  errors: {
    [key: string]: string[];
  };
}

export interface ValidationError {
  message: string;
  errors: {
    [key: string]: string[];
  };
}
