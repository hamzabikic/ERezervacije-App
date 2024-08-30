import {Component, HostListener, OnInit} from '@angular/core';
import {MyAuth} from "../Servisi/MyAuth";
import {HttpClient} from "@angular/common/http";
import {GostManagerResponse, KorisnikEditResponse, KorisnikManagerEdit} from "../KorisnikKlase/ManagerKlase";
import {MojConfig} from "../Servisi/MojConfig";
import {Grad} from "../KorisnikKlase/KorisnikResponse";
import {isNullOrEmpty} from "../Helpers/StringHelper";

@Component({
  selector: 'app-manager-gosti',
  templateUrl: './manager-gosti.component.html',
  styleUrls: ['./manager-gosti.component.css']
})
export class ManagerGostiComponent implements OnInit {
  imePrezime = "";
  gosti: GostManagerResponse[] = [];
  dialog_edit = false;
  editovanje = false;
  naslov = "detalji";
  trenutni_gost: GostManagerResponse | undefined;
  ime = "";
  prezime = "";
  datumRodjenja: string = "";
  brojTelefona = "";
  email = "";
  username = "";
  gradId = 0;
  gradovi: Grad[] = [];
  datumRegistracije = "";
  brojRezervacija = "";
  korisnikId = 0;
  greska = "";
  upozorenje_brisanje = false;
  novaLozinka_dialog = false;
  slikaURL = "";
  promijenjena = false;
  sirina:number=0;

  constructor(private auth: MyAuth, private http: HttpClient) {
  }

  async ngOnInit() {
    await this.getGradovi();
    await this.getGosti();
  }

  ucitajSliku() {
    let reader = new FileReader();
    // @ts-ignore
    let file = document.getElementById("slika").files[0];
    if (!file) {
      this.slikaURL = "";
      this.promijenjena = false;
      return;
    }
    let fileSizeMB = file.size / (1024 * 1024);
    if(fileSizeMB>2) {
      this.slikaURL ="";
      this.promijenjena =false;
      alert("Velicina slike je prevelika! Maksimalna velicina: 2mb");
      return;
    }
    reader.onload = () => {
      this.slikaURL = reader.result!.toString();
      this.promijenjena = true;
    }

    reader.readAsDataURL(file);
  }

  async resetujLozinku(id: number) {
    let res = await this.http.get(MojConfig.adresa_servera + "GenerisiNovuLozinku?korisnikId="
      + id, {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise();
    if (res) {
      this.novaLozinka_dialog = !this.novaLozinka_dialog;
    } else {
      alert("Neuspjesno resetovanje lozinke!");
    }
  }

  provjeraPolja() {
    return isNullOrEmpty(this.ime) || isNullOrEmpty(this.prezime) || isNullOrEmpty(this.username) ||
      isNullOrEmpty(this.email) || isNullOrEmpty(this.brojTelefona) || this.gradId == 0;
  }

  async sacuvaj() {
    if (this.provjeraPolja()) {
      this.greska = "Sva polja su obavezna!";
      return;
    }
    let obj: KorisnikManagerEdit = {
      ime: this.ime,
      prezime: this.prezime,
      datumRodjenja: this.datumRodjenja,
      brojTelefona: this.brojTelefona,
      korisnikId: this.korisnikId,
      email: this.email,
      gradId: this.gradId,
      username: this.username
    };
    let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera + "EditGostManager", obj,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise();
    if (res?.editovan!) {
      if (this.promijenjena) {
        let obj = {
          base64String: this.slikaURL,
          korisnikId: this.korisnikId
        };
        let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera + "SendProfilna", obj,
          {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise();
        if (!res?.editovan) {
          alert("Ucitavanje slike nije uspjelo! Greska: " + res?.greska);
        }
        else {
          location.reload();
        }
      }
      this.dialog_edit = !this.dialog_edit;
      await this.getGosti();
    } else {
      this.greska = res?.greska!;
    }
  }

  popuni(gost: GostManagerResponse) {
    this.ime = gost.ime;
    this.prezime = gost.prezime;
    this.email = gost.email;
    this.brojTelefona = gost.brojTelefona;
    this.username = gost.username;
    this.brojRezervacija = gost.brojRezervacija.toString();
    this.gradId = gost.gradID;
    this.datumRodjenja = gost.datumRodjenja.toString().split("T")[0];
    this.datumRegistracije = gost.datumRegistracije.toString().split("T")[0];
    this.brojRezervacija = gost.brojRezervacija.toString();
    this.korisnikId = gost.korisnikID;
  }

  async obrisiGosta(id: number) {
    let res = await this.http.get(MojConfig.adresa_servera + "IzbrisiKorisnikManager?id=" + id,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise();
    if (res) {
      this.upozorenje_brisanje = !this.upozorenje_brisanje;
      await this.getGosti();
    } else {
      alert("Neuspjesno brisanje gosta!");
    }
  }

  async getGradovi() {
    this.gradovi = await this.http.get<Grad[]>(MojConfig.adresa_servera + "GetGradovi").toPromise() as Grad[];
  }

  async getGosti() {
    if (this.imePrezime == "") {
      this.gosti = await this.http.get<GostManagerResponse[]>(MojConfig.adresa_servera + "GetGostiManager",
        {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise() as GostManagerResponse[];
      return;
    } else {
      this.gosti = await this.http.get<GostManagerResponse[]>(MojConfig.adresa_servera + "GetGostiManager?imePrezime=" +
        this.imePrezime,
        {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise() as GostManagerResponse[];
    }
  }

  protected readonly MojConfig = MojConfig;
  provjera() {
     this.slika_omogucena = window.innerWidth >1350;
    this.rezervacije_omogucene = window.innerWidth>1280;
    this.datum_omogucen = window.innerWidth > 1130;
    this.vidljiva_tabela = window.innerWidth > 970;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event:Event) {
    this.provjera();
  }

  slika_omogucena= window.innerWidth>1350;
  rezervacije_omogucene = window.innerWidth>1280;
  datum_omogucen = window.innerWidth > 1130;
  vidljiva_tabela = window.innerWidth > 970;
}

