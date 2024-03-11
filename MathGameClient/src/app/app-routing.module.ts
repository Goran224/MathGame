import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthGuard } from './_guards/auth.guard';
import { HomeComponent } from './components/home/home.component';
import { GameComponent } from './components/game/game.component';
import { GameSessionGuard } from './_guards/GameSession.guard';

const routes:  Routes = [
  {path: '', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'home', component: HomeComponent, canActivate: [AuthGuard]},
  {path: 'game', component: GameComponent, canActivate: [AuthGuard, GameSessionGuard]},
  {path: '**', component: LoginComponent, pathMatch: 'full'},
];

@NgModule({
  imports: [CommonModule, BrowserAnimationsModule,ToastrModule.forRoot(), RouterModule.forRoot(routes)],
  exports: [RouterModule, FormsModule],
})
export class AppRoutingModule { }