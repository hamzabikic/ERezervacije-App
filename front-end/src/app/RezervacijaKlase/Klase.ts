export interface Stol {
  stolID: number;
  brojStola: number;
  jeUnutra: boolean;
  jePusackaZona: boolean;
  brojStolica: number;
}
export interface StoloviResponse {
  stolovi:Stol[]
}
export interface StoloviRequest {
  datumVrijeme: string;
  trajanje: number;
}
export interface RezervacijaPost {
  datumVrijeme: string,
  trajanje: number;
  posebneZelje: string;
  gostID: number;
  stolovi: number[];
}
export interface RezervacijaResponse {
  rezervacijaID:number;
  greska:string;
}
export interface ManuelnaRezPost {
  datumVrijeme:string,
  trajanje: number;
  posebneZelje: string;
  imePrezime: string;
  brojTelefona: string;
  stolovi: number[];
}
export interface ZabranaStolaPost {
  stolovi: number[];
  pocetakVrijeme: string;
  krajVrijeme: string;
}
export interface ZabranaResponse {
  zabranaId: number;
  stolovi: Stol[];
  pocetakDatum: string;
  vrijemeOd: string;
  krajDatum: string;
  vrijemeDo: string;
}
export interface StoloviRequestZabrana {
  datumPocetak:string,
  datumKraj:string
}

