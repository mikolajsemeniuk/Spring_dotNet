# Rxjs map, pipe, operators
in `workshop.component.ts`
```ts
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Post } from 'src/app/models/post.model';

@Component({
  selector: 'app-workshop',
  templateUrl: './workshop.component.html',
  styleUrls: ['./workshop.component.scss']
})
export class WorkshopComponent implements OnInit {
  
  readonly url: string = 'https://jsonplaceholder.typicode.com'

  posts$: Observable<Post[]>
  posts: Post[]
  pipePosts: Post[]
  subscription: Subscription

  constructor(private httpClient: HttpClient) { }

  ngOnInit(): void { 
    this.getHttpModuleData()
    this.subscription = this.getSubscribeData()
    this.getPipeData()
  }

  getHttpModuleData(): void {
    let headers = new HttpHeaders().set('Authorization', 'auth-token')
    this.posts$ = this.httpClient.get<Post[]>(`${this.url}/posts?_start=0&_limit=3`, { headers })
  }

  getSubscribeData(): Subscription {
    return this.httpClient.get<Post[]>(`${this.url}/posts?_start=0&_limit=3`).subscribe(
      posts => {
        this.posts = posts
      },
      err => {
        console.log(err)
      },
      () => {
        console.log('done')
      }
    )
  }

  getPipeData(): any {
    return this.httpClient.get<Post[]>(`${this.url}/posts?_start=0&_limit=3`).pipe(
      // filter data
      map(
        (posts: Post[]) => posts.filter(
          (post: Post) => post.id % 2 == 0
        )
      ),
      // modify original data, do not use newItem if you would like return
      // array of attributes instead of regular objects
      map(
        (posts: Post[]) => posts.map(
          (post: Post) => {
            let newItem = { ...post }
            newItem.userId = post.userId * 20
            return newItem;
          }
        )
      ),
      // does not change original data only for read values
      tap(
        (posts: Post[]) => posts.map(
          (post: Post) => post.userId * 20
        )
      )
      // instead of map we could also use mergeMap, mergeAll, switchMap, switchAll
      // the main difference based on moment of subscribing the data
    )
    .subscribe(
      (posts: Post[]) => {
        this.pipePosts = posts
      },
      err => {
        console.log(err)
      },
      () => {
        console.log('done')
      }
    )
  }

  ngOnDestroy() {
    // Unsubscribe when the component is destroyed
    this.subscription.unsubscribe()
  }
}
```
in `post.model.ts`
```ts
export class Post {
    id: number
    title: string
    body: string
    userId: number
}
```
in `workshop.component.template`
```html

<p>HttpClient</p>

<!-- 
    use "async" if your data is array of Observable
    and yu have to unwrap it for instance using 
    when `posts` = `posts: any` 
-->
<div *ngFor="let post of posts$ | async">
    {{ post | json }}
</div>

<p>Subscribe</p>

<div *ngFor="let post of posts">
    {{ post | json }}
</div>

<p>Pipe</p>

<div *ngFor="let post of pipePosts">
    {{ post | json }}
</div>
```
