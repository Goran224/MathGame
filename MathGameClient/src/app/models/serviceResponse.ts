export interface ServiceResponse<T> {
    data?: T;
    success: boolean;
    errorMessage?: string;
  }