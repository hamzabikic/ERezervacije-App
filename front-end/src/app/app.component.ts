import {Component, HostListener, OnDestroy, OnInit} from '@angular/core';
import {MyAuth} from "./Servisi/MyAuth";
import {Router} from "@angular/router";
import {AutentifikacijaEndpoints} from "./Endpoints/AutentifikacijaEndpoints";
import {OdjavaResponse} from "./AutentifikacijaKlase/LoginRequest";
import {MojConfig} from "./Servisi/MojConfig";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  mali_navbar = window.innerWidth<800 ;
  otvoren_mali = false;
  constructor( protected auth:MyAuth,private router:Router,private endpoints:AutentifikacijaEndpoints) {
  }
    ngOnInit(){
    }
   osvjezi() {
    location.reload();
  }
  mali_nav_style() {
    if (!this.mali_navbar) {
      return {
        "width": "fit-content"
      };
    }

    if (this.otvoren_mali) {
      return {
        "width": "50%"
      };
    } else {
      return {
        "width": "0"
      };
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event:Event) {
    this.mali_navbar = window.innerWidth<800 ;
  }
    async odjava() {
    let odjava = await this.endpoints.Logout().toPromise();
    if(odjava!.odjavljen){
    localStorage.removeItem("my-token");
    this.otvoren_mali=false;
    this.router.navigate(["/login"]); }
    else {
      alert("Neuspjela odjava!");
    }
    }

  protected readonly MojConfig = MojConfig;
}
