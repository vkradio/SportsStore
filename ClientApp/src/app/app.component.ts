import { Component } from '@angular/core';
import { ErrorHandlerService } from './errorHandler.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private lastError: string[];

  constructor(errorService: ErrorHandlerService) {
    errorService.errors.subscribe(error => {
      this.lastError = error;
    });
  }

  get error() {
    return this.lastError;
  }

  clearError() {
    this.lastError = null;
  }
}
