import { Component, OnInit } from '@angular/core';
import {isNullOrEmpty} from "../Helpers/StringHelper";
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {MojConfig} from "../Servisi/MojConfig";

@Component({
  selector: 'app-verifikacija-email',
  templateUrl: './verifikacija-email.component.html',
  styleUrls: ['./verifikacija-email.component.css']
})
export class VerifikacijaEmailComponent implements OnInit {
  obavjestenje="";
  key="";
  uspjesno = false;

  constructor(private http:HttpClient, private route: ActivatedRoute) {
    this.key = this.route.snapshot.params["key"];
  }

  async ngOnInit() {
    await this.verifikujEmailNalog();
  }
  async verifikujEmailNalog () {
    if(isNullOrEmpty(this.key)) {
      this.obavjestenje="Greška! Neuspješna verifikacija e-mail naloga!";
    }
    this.uspjesno = await this.http.get(MojConfig.adresa_servera+"VerifikujEmail?key="+this.key).toPromise() as boolean;
    if(this.uspjesno) {
      this.obavjestenje="Uspješna verifikacija e-mail naloga!";
    }
    else {
      this.obavjestenje="Greška! Neuspješna verifikacija e-mail naloga!";
    }
  }
  boolstyle () {
    if(!this.uspjesno) {
      return {
        "color":"darkred"
      };
    }
    else {
      return {
      "color" :"green" };
    }
  }

}
