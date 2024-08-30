import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {inject, Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {MyAuth} from "./MyAuth";

@Injectable()
export class MojNalogManagerProvjera implements CanActivate {
  constructor(private auth: MyAuth, private router:Router) {

  }
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    if(!this.auth.isPrijavljen() || !this.auth.isManager()){
      this.router.navigate(["/login"])
      return false;
    }
    return true;
  }

}
