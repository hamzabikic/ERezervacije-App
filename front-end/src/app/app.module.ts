import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import {FormsModule} from "@angular/forms";
import {RouteReuseStrategy, RouterModule} from '@angular/router';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import { LoginComponent } from './login/login.component';
import { RegistracijaComponent } from './registracija/registracija.component';
import {MyAuth} from "./Servisi/MyAuth";
import {AutentifikacijaEndpoints} from "./Endpoints/AutentifikacijaEndpoints";
import { MojNalogGostComponent } from './moj-nalog-gost/moj-nalog-gost.component';
import {LoginProvjera} from "./Servisi/LoginProvjera";
import {MojNalogGostProvjera} from "./Servisi/MojNalogGostProvjera";
import { KreiranjeRezervacijeComponent } from './kreiranje-rezervacije/kreiranje-rezervacije.component';
import { MojNalogHostesaComponent } from './moj-nalog-hostesa/moj-nalog-hostesa.component';
import {MojNalogHostesaProvjera} from "./Servisi/MojNalogHostesaProvjera";
import { MojNalogManagerComponent } from './moj-nalog-manager/moj-nalog-manager.component';
import { ManagerGostiComponent } from './manager-gosti/manager-gosti.component';
import { ManagerHosteseComponent } from './manager-hostese/manager-hostese.component';
import { ManagerRezervacijeComponent } from './manager-rezervacije/manager-rezervacije.component';
import { VerifikacijaEmailComponent } from './verifikacija-email/verifikacija-email.component';
import {MojNalogManagerProvjera} from "./Servisi/MojNalogManagerProvjera";
import { NovaLozinkaComponent } from './nova-lozinka/nova-lozinka.component';
import { ZaboravljenaLozinkaComponent } from './zaboravljena-lozinka/zaboravljena-lozinka.component';
import { StoloviComponent } from './stolovi/stolovi.component';
import {DatePipe} from "@angular/common";


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegistracijaComponent,
    MojNalogGostComponent,
    KreiranjeRezervacijeComponent,
    MojNalogHostesaComponent,
    MojNalogManagerComponent,
    ManagerGostiComponent,
    ManagerHosteseComponent,
    ManagerRezervacijeComponent,
    VerifikacijaEmailComponent,
    NovaLozinkaComponent,
    ZaboravljenaLozinkaComponent,
    StoloviComponent
  ],
    imports: [
        BrowserModule,
        FormsModule,
      HttpClientModule,
      RouterModule.forRoot([
        {path:"login", component:LoginComponent, canActivate:[LoginProvjera]},
        {path:"registracija", component:RegistracijaComponent, canActivate:[LoginProvjera]},
        {path:"moj-nalog-gost",component:MojNalogGostComponent,canActivate:[MojNalogGostProvjera]},
        {path:"kreiranje-rezervacije", component:KreiranjeRezervacijeComponent, canActivate: [MojNalogGostProvjera]},
        {path:"moj-nalog-hostesa", component:MojNalogHostesaComponent, canActivate:[MojNalogHostesaProvjera]},
        {path:"moj-nalog-manager", component:MojNalogManagerComponent, canActivate:[MojNalogManagerProvjera]},
        {path:"manager/gosti", component:ManagerGostiComponent, canActivate:[MojNalogManagerProvjera]},
        {path:"manager/hostese", component:ManagerHosteseComponent, canActivate:[MojNalogManagerProvjera]},
        {path:"manager/rezervacije", component:ManagerRezervacijeComponent, canActivate:[MojNalogManagerProvjera]},
        {path:"verifikacija-email/:key", component:VerifikacijaEmailComponent},
        {path:"nova-lozinka/:key", component:NovaLozinkaComponent, canActivate:[LoginProvjera]},
        {path:"zaboravljena-lozinka", component:ZaboravljenaLozinkaComponent, canActivate:[LoginProvjera]},
        {path:"manager/admin-panel", component:StoloviComponent, canActivate:[MojNalogManagerProvjera]},
        {path:"", redirectTo:"login", pathMatch:"full"},
        {path:"**", redirectTo:"login", pathMatch:"full"},
      ],{useHash:true})
    ],
  providers: [DatePipe,MyAuth,AutentifikacijaEndpoints,LoginProvjera,MojNalogGostProvjera,MojNalogHostesaProvjera, MojNalogManagerProvjera],
  bootstrap: [AppComponent]
})
export class AppModule { }
