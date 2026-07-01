import { HttpInterceptorFn } from '@angular/common/http';

export const apiUrlInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith('/api')) {
    const newUrl = `http://localhost:5054${req.url}`;
    const newReq = req.clone({ url: newUrl });
    return next(newReq);
  }
  return next(req);
};