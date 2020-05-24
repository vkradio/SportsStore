import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-blazor',
  templateUrl: 'blazorLoader.component.html'
})
export class BlazorLoaderComponent implements OnInit {
  template: any = '';

  ngOnInit() {
    if (!document.getElementById('blazorScript')) {
      const scriptElem = document.createElement('script');
      scriptElem.type = 'text/javascript';
      scriptElem.id = 'blazorScript';
      scriptElem.src = '_framework/blazor.webassembly.js';
      document.getElementsByTagName('head')[0].appendChild(scriptElem);
    }
  }
}
