using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for JOffer
/// </summary>
public class JOffer
{
    public string supplier;
    public string gmSupplier;
    public string crSupplier;
    public string bestSupplier;
    public float price;
    public float gmPrice;
    public float crPrice;
    public float bestPrice;
    public string siteName;
    public string transmission;
    public string category;
    public string seats;

    public Supplier Other = new Supplier();
    public Supplier GMotion = new Supplier();
    public Supplier CRent = new Supplier();
    public Supplier Best = new Supplier();

    public JOffer(String splr, String prc)
    {
        Debug.WriteLine(splr);
        switch (splr.ToUpper())
        {
            case "GREEN MOTION":
            case "GREENMOTION":
           case "GREEN_MOTION":
                SetGM(splr);
                SetGMPrice(prc);
                //GMotion = new Supplier { name = splr, price = float.Parse(Regex.Match(prc.Replace(',', '.'), "\\d+\\.?\\d+").Value) };
                break;
            case "CARSRENT":
            case "CARS RENT":
            case "CARS_RENT":
                SetCR(splr);
                SetCRPrice(prc);
              //  CRent = new Supplier { name = splr, price = float.Parse(Regex.Match(prc.Replace(',', '.'), "\\d+\\.?\\d+").Value) };
                break;
            case "":
                //Best = new Supplier { name = splr, price = float.Parse(Regex.Match(prc.Replace(',', '.'), "\\d+\\.?\\d+").Value) };
                SetBest("MyB");
                SetBestPrice(prc);
                break;
            default:
                SetSupplier(splr);
                SetPrice(prc);
                //Other = new Supplier { name = splr, price = float.Parse(Regex.Match(prc.Replace(',', '.'), "\\d+\\.?\\d+").Value) };
                break;
        }
    }

    private void SetBest(string splr)
    {
        bestSupplier = "Best";
    }

    public JOffer(String splr, float prc)
    {
        switch (splr.ToUpper())
        {
            case "GREEN MOTION":
            case "GREEN_MOTION":
                SetGM(splr);
                SetGMPrice(prc);
                GMotion = new Supplier { name = splr, price = prc };
                break;
            case "CARSRENT":
            case "CARS RENT":
            case "CARS_RENT":
                SetCR(splr);
                SetCRPrice(prc);
                CRent = new Supplier { name = splr, price = prc };
                break;
            case "":
                SetBest("MyB");
                SetBestPrice(prc);
                break;
            default:
                SetSupplier(splr);
                SetPrice(prc);
                Other = new Supplier { name = splr, price = prc };
                break;
        }
    }

    public JOffer() { }

    public void SetSupplier(String splr)
    {
        supplier = splr;
    }

    public void SetCategory(string ctr)
    {
        category = MapCategory(ctr);
    }

    public string GetSiteName()
    {
        return siteName;
    }

    public void SetSiteName(string site)
    {
        siteName = site;
    }

    public String GetSupplier()
    {
        return this.supplier;
    }

    public void SetPrice(string price)
    {
        this.price = float.Parse(Regex.Match(price.Replace(',', '.'), "\\d+\\.?\\d+").Value);
    }

    public void SetPrice(float price)
    {
        this.price = price;
    }

    public float GetPrice()
    {
        return this.price;
    }

    public void SetGM(String splr)
    {
        this.gmSupplier = splr;
    }

    public String GetGM()
    {
        return this.gmSupplier;
    }

    public void SetGMPrice(String price)
    {
        this.gmPrice = float.Parse(Regex.Match(price.Replace(',', '.'), "\\d+\\.?\\d+").Value);
    }

    public void SetGMPrice(float price)
    {
        this.gmPrice = price;
    }

    public float GetGMPrice()
    {
        return this.gmPrice;
    }

    public String GetBest()
    {
        return this.bestSupplier;
    }

    public void SetBestPrice(String price)
    {
        this.bestPrice = float.Parse(Regex.Match(price.Replace(',', '.'), "\\d+\\.?\\d+").Value);
    }

    public void SetBestPrice(float price)
    {
        this.bestPrice = price;
    }

    public float GetBestPrice()
    {
        return this.bestPrice;
    }

    public void SetCR(String splr)
    {
        this.crSupplier = splr;
    }

    public String GetCR()
    {
        return this.crSupplier;
    }

    public void SetCRPrice(String price)
    {
        this.crPrice = float.Parse(Regex.Match(price.Replace(',', '.'), "\\d+\\.?\\d+").Value);
    }

    public void SetCRPrice(float price)
    {
        this.crPrice = price;
    }

    public float GetCRPrice()
    {
        return this.crPrice;
    }

    public String toString()
    {
        return supplier + " " + price + " " + category;
    }

    protected string IsEmpty(string strToCheck)
    {
        if (strToCheck.Equals(""))
            return strToCheck;
        else
            return strToCheck + "\n";
    }

    protected string IsEmptySupplier(Supplier sup)
    {
        if (sup.name != null)
        {
            if (sup.name.Equals(""))
                return "-" + " " + sup.price + "\n";
            else
                return sup.name.ToLower().Substring(0, 4) + " " + sup.price + "\n";
        }
        return "";
    }
    //public String GetOffer()
    //{   string offer = validate(supplier, price);
    //    string gmOffer = validate(gmSupplier, gmPrice);
    //    string crOffer = validate(crSupplier, crPrice);

    //    List<Supplier> supplierList = new List<Supplier> { GMotion, CRent, Other, Best };

    //    supplierList.OrderBy(o => o.price);

    //    string offerStr = String.Empty;

    //    foreach(Supplier sup in supplierList){
    //        if(sup.price > 0)
    //            offerStr += sup + "\n";
    //    }
    //    return offerStr;
    //}

    public String GetOffer1()
    {// a price b gnPrice c crPrice
        string offer = validate(supplier, price);//0
        string gmOffer = validate(gmSupplier, gmPrice);//1
        string crOffer = validate(crSupplier, crPrice);//2

        string output = string.Empty;

        if (price < gmPrice)
        {
            if (price < crPrice)
            {
                output += IsEmpty(offer);
                if (gmPrice < crPrice)
                {
                    return output += IsEmpty(gmOffer) + crOffer;
                }
                else
                    return output += IsEmpty(crOffer) + gmOffer;
            }
            else
            {
                return IsEmpty(crOffer) + IsEmpty(offer) + gmOffer;
            }
        }
        else
        {
            if (gmPrice <= crPrice)
            {
                if (gmPrice <= price)
                {
                    output += IsEmpty(gmOffer);
                    if (crPrice < price)
                        return output += IsEmpty(crOffer) + offer;
                    else
                        return output += IsEmpty(offer) + crOffer;
                }
            }
            else
            {
                if (crPrice < price)
                {
                    if (crPrice < gmPrice)
                    {
                        output += IsEmpty(crOffer);
                        if (gmPrice < price)
                            return output += IsEmpty(gmOffer) + offer;
                        else
                            return output += IsEmpty(offer) + gmOffer;
                    }
                    else
                        return offer;
                }

            }
        }
        return "";
    }

    public String GetOffer()
    {// a price b gnPrice c crPrice
        string offer = validate(supplier, price);//0
        string gmOffer = validate(gmSupplier, gmPrice);//1
        string crOffer = validate(crSupplier, crPrice);//2

        List<Supplier> suppliers = new List<Supplier>() { new Supplier { name = supplier, price = price }, new Supplier { name = gmSupplier, price = gmPrice }, new Supplier { name = crSupplier, price = crPrice }, new Supplier { name = bestSupplier, price = bestPrice } };
        string output = string.Empty;
        foreach (Supplier sup in suppliers.OrderBy(t => t.price))
        {
            output += IsEmptySupplier(sup);
        }
        return output;
    }

    private string MapCategory(string category)
    {
        switch (category)
        {
            case "MCMR":
            case "MCMN":
            case "MDMR":
                return "MiniM";
            case "EDMR":
            case "EDMN":
            case "ECMN":
            case "ECMR":
                return "EconomyM";
            case "EDAR":
            case "ECAR":
            case "ECAN":
            case "EDAN":
                return "EconomyA";
            case "CDMR":
            case "CCMR":
            case "CDMN":
            case "CCMN":
                return "CompactM";
            case "CDAR":
            case "CCAR":
            case "CDAN":
            case "CCAN":
                return "CompactA";
            case "IDMR":
            case "ICMR":
                return "IntermediateM";
            case "IDAR":
            case "ICAR":
                return "IntermediateA";
            case "SDMR":
            case "SCMR":
                return "StandardM";
            case "SDAR":
            case "SCAR":
                return "StandardA";
            case "SWMR":
            case "IWMR":
            case "CWMR":
                return "EstateM";
            case "SWAR":
            case "IWAR":
            case "CWAR":
                return "EstateA";
            case "IFMR":
            case "IFMN":
            case "SFMR":
            case "CFMR":
            case "EFMR":
                return "SUVM";
            case "IFAR":
            case "IFAN":
            case "SFAR":
            case "CFAR":
            case "EFAR":
                return "SUVA";
            case "PWMR":
            case "SVMR":
                return "People CarrierM";
            default:
                //System.Diagnostics.Debug.WriteLine(category);
                break;
        }
        return "skip";
    }

    private string validate(string supplier, float price)
    {
        if (price > 10000 || price == 0)
            return "";
        if (supplier != null && supplier.Length < 3 && price > 0)
            supplier = "best";
        return supplier != null && supplier.Length > 3 ? char.ToUpper(supplier[0]).ToString() + char.ToLower(supplier[1]).ToString() + char.ToLower(supplier[2]).ToString() + " " + price : supplier + " " + price;
    }

    public class Supplier
    {
        public string name;
        public float price;

        //public override string ToString()
        //{
        //    if (price > 10000 || price == 0)
        //        return "";
        //    return string.IsNullOrEmpty(name) && name.Length > 3 ? name.Substring(0, 2) + " " + price : name + " " + price;
        //}
    }
}


