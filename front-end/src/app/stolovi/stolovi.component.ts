import {Component, HostListener, OnInit} from '@angular/core';
import {
  RezervacijaResponse,
  Stol,
  StoloviRequest,
  StoloviRequestZabrana,
  StoloviResponse,
  ZabranaResponse,
  ZabranaStolaPost
} from "../RezervacijaKlase/Klase";
import {HttpClient} from "@angular/common/http";
import {MyAuth} from "../Servisi/MyAuth";
import {MojConfig} from "../Servisi/MojConfig";
import {formatirajBroj} from "../Helpers/StringHelper";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'app-stolovi',
  templateUrl: './stolovi.component.html',
  styleUrls: ['./stolovi.component.css']
})
export class StoloviComponent implements OnInit {
  zabrane: ZabranaResponse[] = [];
  greska="";
  odSati = 0;
  doSati = 0;
  slobodniStolovi :Stol[] = [];
  datumPocetak="";
  datumKraj = "";
  odLista=[];
  doLista=[];
  odabraniStolovi:number[] = [];
  dialog_novo=false;
  zabrana:ZabranaResponse|undefined;
  dialog_detalji = false;
  vidljiviStolovi = window.innerWidth > 1100;
  vidljiva_tabela = window.innerWidth>549;
  svi_stolovi = false;

  constructor(private http: HttpClient, private auth:MyAuth, private datePipe:DatePipe) { }

  async ngOnInit(){
    this.podesiVrijeme();
    this.popuniListu(this.odLista, 9, 23);
    this.popuniListu(this.doLista, 9, 23);
    await this.getZabrane();
    await this.preuzmiStolove();
  }
  podesiVrijeme () {
    let currentTime = this.datePipe.transform(new Date(), 'yyyy-MM-dd HH:mm:ss', 'Europe/Sarajevo');
    let datum = new Date(currentTime!);
    this.odSati = ((datum.getHours()+1) >22 || (datum.getHours()+1) <9) ? 9 : datum.getHours()+1;
    this.doSati = ((datum.getHours()+1) >22 || (datum.getHours()+1) <9) ? 10 : datum.getHours()+2;
    if((datum.getHours()+1) >22) {
      datum = new Date(datum.setDate(datum.getDate()+1));
    }
    this.datumPocetak = datum.getFullYear() +"-"+formatirajBroj(datum.getMonth()+1)+"-" +
      formatirajBroj(datum.getDate());
    this.datumKraj= this.datumPocetak;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event:Event) {
    this.vidljiviStolovi= window.innerWidth>1100;
    this.vidljiva_tabela= window.innerWidth>549;
  }
  async getZabrane() {
    this.zabrane = await this.http.get<ZabranaResponse[]>(MojConfig.adresa_servera+"GetZabrane",
      {headers:{"my-token": this.auth.getToken()?.token!}}).toPromise() as ZabranaResponse[];

  }
  getStoloviBrojevi (lista:Stol[]) {
    return lista.map(s=> s.brojStola);
  }
  popuniListu(lista: number[], odSati: number, doSati: number) {
    for (let i = odSati; i <= doSati; i++) {
      lista.push(i);
    }
  }
  async preuzmiStolove() {
    console.log(typeof(this.odSati));
    console.log(typeof(this.doSati));
    this.odabraniStolovi = [];
    if(this.datumPocetak== this.datumKraj && this.odSati >= this.doSati) {
      this.greska="Nepravilno postavljena satnica!";
      this.slobodniStolovi= [];
      return;
    }
    else {
      this.greska="";
    }
    let obj: StoloviRequestZabrana = {
      datumPocetak: this.datumPocetak + "T" +formatirajBroj(this.odSati) + ":00:00.000Z",
      datumKraj: this.datumKraj + "T" + formatirajBroj(this.doSati) + ":00:00.000Z",
    };
    this.slobodniStolovi= (await this.http.post(MojConfig.adresa_servera + "GetSlobodniStoloviZabrana", obj,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise() as StoloviResponse).stolovi;
  }
  async posaljiZabranu() {
    if(this.odSati >= this.doSati && this.datumPocetak == this.datumKraj) {
      this.greska="Nepravilno postavljena satnica!";
      return;
    }
    if(this.svi_stolovi) {
      this.odabraniStolovi = this.slobodniStolovi.map(s=> s.stolID);
    }
    else {
      if(this.odabraniStolovi.length==0) {
        this.greska="Niste odabrali stolove!";
        return;
      }
    }
    let obj : ZabranaStolaPost = {
      stolovi:this.odabraniStolovi,
      pocetakVrijeme: this.datumPocetak + "T" + formatirajBroj(this.odSati) + ":00:00.000Z",
      krajVrijeme: this.datumKraj + "T" + formatirajBroj(this.doSati) + ":00:00.000Z"
    };
    let res :RezervacijaResponse = await this.http.post<RezervacijaResponse>(MojConfig.adresa_servera+"ZabranaStolova",obj,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as RezervacijaResponse;
    if(res.rezervacijaID!=0) {
      alert("Uspjesno dodano ogranicenje!");
      await this.getZabrane();
      this.dialog_novo= !this.dialog_novo;
    }
    else {
      this.greska=res.greska;
    }
  }
  async obrisiOgranicenje(id:number) {
    let res :boolean = await this.http.get<boolean>(MojConfig.adresa_servera+"IzbrisiZabranu?id="+id,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as boolean;
    if(res){
      await this.getZabrane();
    }
    else {
      alert("Neuspjesno brisanje!");
    }
  }
  async ocistiPolja() {
    this.podesiVrijeme();
    this.svi_stolovi=false;
    this.odabraniStolovi= [];
    this.greska="";
    await this.preuzmiStolove();
  }

  protected readonly formatirajBroj = formatirajBroj;
}
