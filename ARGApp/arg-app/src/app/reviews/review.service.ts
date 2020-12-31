import { Injectable } from '@angular/core';
import { IReview } from './review.model';
import { Observable, throwError } from 'rxjs';
import { map, filter, catchError, tap } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  private reviewUrl: string = 'https://xog-trial-arg.azurewebsites.net/api/generate';
  constructor(private _http: HttpClient) {

  }

  getReview(): Observable<IReview>{
    return this._http.get<IReview>(this.reviewUrl).pipe(
      tap(data => console.log('Error'), catchError(this.handleError)));
  }

  private handleError(err: HttpErrorResponse) {

    let errorMessage = '';

    if (err.error instanceof ErrorEvent) {
        errorMessage = `An error occurred: ${err.error.message}`;
    }
    else {
      errorMessage = `Server returned code: ${err.status}. error message is: ${err.message}`;
    }

    console.error(errorMessage);
    return throwError(errorMessage);
  }
}
