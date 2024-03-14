import { Component, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';
import { PlayerService } from './services/player.service';
import { AuthGuard } from './_guards/auth.guard';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'MathGameApp';
  constructor(public playerService: PlayerService, private router: Router, private authGuard: AuthGuard) {}
  ngOnInit(): void {
    this.playerService.isAuthenticated().subscribe(authenticated => {
      if (authenticated) {
        const canActivate = this.authGuard.canActivate();
        if (canActivate) {
          const routeName = this.getCurrentRouteName();
          const routesToReRoute = routeName && routeName !== '' &&
                                  routeName !== 'home' &&
                                  routeName !== 'register';


          if (routesToReRoute && this.getPredefinedRoutes().includes(routeName)) {

            this.router.navigate([routeName]);

          } else {
            this.router.navigateByUrl('home')
          }
          this.playerService.setCurrentPlayer(true);
        }
      }
    });
  }

  getCurrentRouteName(): string {
    const currentUrl = window.location.href;
    const routeParts = currentUrl.split('/'); 
    return routeParts[routeParts.length - 1];
  }
  getPredefinedRoutes(): (string | undefined)[]  {
    const routes: Route[] = this.router.config;
    const predefinedRoutes = routes.map(route => route.path);
    return  predefinedRoutes.filter(route => route !== '**' && route !== ''); 
  }
}
