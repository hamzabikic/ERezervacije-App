import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {Observable} from "rxjs";
import {Injectable} from "@angular/core";
import {MyAuth} from "./MyAuth";
@Injectable()
export class MojNalogGostProvjera implements CanActivate {
  constructor(private auth:MyAuth,private router:Router) {
  }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
      if(this.auth.isPrijavljen() && this.auth.isGost()) {
        return true;
      }
      this.router.navigate(["/login"]);
      return false;
    }

}
