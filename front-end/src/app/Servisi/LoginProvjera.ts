import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {Observable} from "rxjs";
import {MyAuth} from "./MyAuth";
import {Injectable} from "@angular/core";
@Injectable()
export class LoginProvjera implements CanActivate {
  constructor(private auth:MyAuth,private router:Router) {
  }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
       if(this.auth.isPrijavljen()) {
         if(this.auth.isGost()){
         this.router.navigate(["/moj-nalog-gost"]); }
         else if(this.auth.isHostesa()) {
           this.router.navigate(["/moj-nalog-hostesa"]);
         }
         else {
           this.router.navigate(["/moj-nalog-manager"]);
         }
         return false;
       }
       return true;
    }

}
