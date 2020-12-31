import { Component } from '@angular/core';
import { ReviewService } from './reviews/review.service';
import { OnInit } from '@angular/core';
import { IReview } from './reviews/review.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'arg-app';
  errorMessage: string  = '';
  review: IReview = {
    reviewText: '',
    overall: 0
  };

  constructor(private _reviewService: ReviewService) {

  }

  ngOnInit(): void {
    this.getReview();
  }

  onGenerateClicked(): void {

    this.getReview();
  }

  private getReview() {
    this._reviewService.getReview().subscribe({
      next: review => this.review = review,
      error: err => this.errorMessage = err
    });
  }
}
