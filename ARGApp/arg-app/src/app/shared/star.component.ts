import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';

@Component ({
  selector: 'pm-star',
  templateUrl: './star.component.html',
  styleUrls: ['./star.component.css']
})
export class StarComponent implements OnChanges {
 starWidth: number;
 @Input() rating: number;

 constructor () {

 }

  ngOnChanges(): void {
    this.starWidth = this.rating * 75 / 5;
  }
}
