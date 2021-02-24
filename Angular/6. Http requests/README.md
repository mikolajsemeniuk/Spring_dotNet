# Http requests
* Import module
* Declare models
* Send requests

### Import module
in `app.module.ts`
```ts
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { HttpClientModule } from '@angular/common/http'; // ADD THIS

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { FirstComponent } from './components/first/first.component';
import { SecondComponent } from './components/second/second.component';
import { ThirdComponent } from './components/third/third.component';
import { WorkshopComponent } from './components/workshop/workshop.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FirstComponent,
    SecondComponent,
    ThirdComponent,
    WorkshopComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,

    HttpClientModule // ADD THIS
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```
### Declare models
```sh
# with tests
ng generate class models/Post --type=model
# without tests
ng generate class models/Post --type=model --skipTests=true
```
