using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for JOffer
/// </summary>
public class JOffer
{
    private string supplier;
    private string gmSupplier;
    private string crSupplier;
    private float price;
    private float gmPrice;
    private float crPrice;
    private string siteName;
    public string transmission;
    public string category;
    public string seats;

    public JOffer(String splr, String prc)
    {
        switch (splr.ToUpper())
        {
            case "GREEN MOTION":
           case "GREEN_MOTION":
                SetGM(splr);
                SetGMPrice(prc);
                break;
            case "CARSRENT":
                SetCR(splr);
                SetCRPrice(prc);
                break;
            default:
                SetSupplier(splr);
                SetPrice(prc);
                break;
        }
    }

    public JOffer(String splr, float prc)
    {
        switch (splr.ToUpper())
        {
            case "GREEN MOTION":
            case "GREEN_MOTION":
                SetGM(splr);
                SetGMPrice(prc);
                break;
            case "CARSRENT":
                SetCR(splr);
                SetCRPrice(prc);
                break;
            default:
                SetSupplier(splr);
                SetPrice(prc);
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

    public String GetOffer1()
    {// a price b gnPrice c crPrice
        string offer = validate(supplier, price);
        string gmOffer = validate(gmSupplier, gmPrice);
        string crOffer = validate(crSupplier, crPrice);

        string offerStr;

        if (price < gmPrice)
            offerStr = offer + "\n" + gmOffer;
        else
            offerStr = gmOffer + "\n" + offer;
        return offerStr;
    }

    protected static string IsEmpty(string strToCheck)
    {
        if (strToCheck.Equals(""))
            return strToCheck;
        else
            return strToCheck + "\n";
    }

    public String GetOffer()
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
                return "SUVM";
            case "IFAR":
            case "IFAN":
            case "SFAR":
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
}


