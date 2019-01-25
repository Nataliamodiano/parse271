using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eligRequest
{
    public class elig271 {

        public class objST {
            public string DateTranCrea { get; set; }
        }

        public class objAaa {
            public string validRequestIndicatorY { get; set; }
            public string validRequestIndicatorN { get; set; }
            public string rejectReasonCode { get; set; }
            public string rejectReason { get; set; }
            public string followUpAction { get; set; }
        }

        public class objNm1 {
            public string lineToWrite { get; set; }
        }

        public class objN3 {
            public string address1 { get; set; }
            public string address2 { get; set; }
        }

        public class objN4 {
            public string cityStateZip { get; set; }
        }

        public class objDmg {
            public string birthDate { get; set; }
            public string gender { get; set; }
        }

        public class objDtp {
            public string dateTimeTotal { get; set; }
        }

        public class objPrv {
            public string prvWrite { get; set; }
        }

        public class objIii {
            public string iiiWrite { get; set; }
        }

        public class objEb {
            public string serviceTypeCodeWrite { get; set; }
            public string eligibilityInfo { get; set; }
            public string coverageLevelCode { get; set; }
            public string insuranceTypeCode { get; set; }
            public string benefitAmount { get; set; }
            public string benefitPercent { get; set; }
            public string timePeriodQual { get; set; }
            public string benefitQuanQual { get; set; }
            public string quantityQualifier { get; set; }
            public string authOrCertIndicator { get; set; }
            public string inPlanNetworkIndicatorY { get; set; }
            public string inPlanNetworkIndicatorN { get; set; }
            public string G0180 { get; set; }
            public string G0179 { get; set; }
            public string prodProc { get; set; }
            public string procedureModifier { get; set; }
            public string diagPointer { get; set; }
        }

        public class objHsd {
            public string quant { get; set; }
        }

        public class objMsg {
            public string freeFormMessageText { get; set; }
        }

        public class objReff {
            public string refWrite { get; set; }
            public string refName { get; set; }
        }

        public class objIns {
            public string insNote { get; set; }
            public string studentStatusCode { get; set; }
            public string handicapIndicator { get; set; }
            public string birthSequenceNumber { get; set; }
        }

        public class objPer {
            public string contactName { get; set; }
            public string communicationNumberQual { get; set; }
            public string communicationNumberQual2 { get; set; }
            public string communicationNumberQual3 { get; set; }
        }
    }

    class Program
    {

        public static string line;
        public static string currentLine;
        public static string currentSeg;
        public static bool inLoopISA;
        public static bool inLoopGS;
        public static bool inLoopST;
        public static bool inLoop2000A;
        public static bool inLoop2100A;
        public static bool inLoop2000B;
        public static bool inLoop2100B;
        public static bool inLoop2000C;
        public static bool inLoop2100C;
        public static bool inLoop2110C;
        public static bool inLoop2115C;
        public static bool inLoop2120C;
        public static bool inLoop2000D;
        public static bool inLoop2100D;
        public static bool inLoop2110D;
        public static bool inLoop2115D;
        public static bool inLoop2120D;
        public static string InsName;        
        public static string traceNumber;
        public static string dependentTaceNumber;
        public static string lastEB = ""; //////////// not sure where this var is initialized
        public static int ebCount = 0;

        public static char segmentDelim;
        public static char elementDelim;
        public static char subElementDelim;
        public static char repetitionDelim;
        public static string s2;

        // Read the file
        public static System.IO.StreamReader file = new System.IO.StreamReader(@"C:\home\271.txt");

        static void Main(string[] args)
        {

            while ((line = file.ReadLine()) != null)
            {
                currentLine = line;

                // determine delimiters
                segmentDelim = System.Convert.ToChar(currentLine.Substring(105, 1)); // ~
                elementDelim = System.Convert.ToChar(currentLine.Substring(3, 1)); // *
                subElementDelim = System.Convert.ToChar(currentLine.Substring(104, 1)); // |
                repetitionDelim = System.Convert.ToChar(currentLine.Substring(82, 1)); // ^

                while (getSeg(currentLine, 0) == "ISA")
                {
                    LoopISA();
                }
            }    

            file.Close();

            // Suspend the screen.  
            System.Console.ReadLine();  
        }

        public static string getNextSeg()
        {
            currentLine = getNextLine();
            currentSeg = getSeg(currentLine, 0);
            return currentSeg;
        }

        public static void LoopISA()
        {
            inLoopISA = true;

            currentSeg = getSeg(currentLine, 0);
            if (currentSeg == "ISA")
            {
                getNextSeg();
                while (currentSeg == "GS")
                {
                    LoopGS();
                }

                getNextSeg();

                if (currentSeg != "IEA") // IEA is the closing segment to ISA
                {
                    Console.WriteLine("Missing IEA"); //////////////////// ERROR
                }  
            }
            inLoopISA = false;
        }

        public static void LoopGS()
        {
            inLoopGS = true;
            if ((getSeg(currentLine, 8) == "004010X092A1") || (getSeg(currentLine,8) == "005010X279A1"))
            {
                getNextSeg();

                while (currentSeg == "ST") 
                {
                    LoopST();
                }
            }

            if (currentSeg != "GE")
            {
                Console.WriteLine("Error missing GE"); //////////////////// ERROR
            }
            inLoopGS = false;
        }

        public static void LoopST()
        {
            inLoopST = true;
            // It should never be a 999 anymore since we're taking care of the 999s on the EmdeonChat before sending it to ParseElig
            getNextSeg();

            if (currentSeg == "BHT") 
            {
                var DateTranCrea = getSeg(currentLine, 4); 
                //addToBothCSVs(header,'DateCreated',outputDate(DateTranCrea)); //////////////////// addToBothCSVs

                elig271.objST objST = new elig271.objST();
                objST.DateTranCrea = outputDate(DateTranCrea);
                Console.WriteLine("                        ELIGIBILITY - " + objST.DateTranCrea); //////////////////// writeln OBJ

                Console.WriteLine(""); //////////////////// writeln

                getNextSeg();
            }
            while ((currentSeg == "HL") && (getSeg(currentLine, 3) == "20")) 
            {
                Loop2000A();
            }
            while ((currentSeg == "HL") && (getSeg(currentLine, 3) == "21"))
            {
                Loop2000B();
            }
            while ((currentSeg == "HL") && (getSeg(currentLine, 3) == "22"))
            {
                Loop2000C();
            }
            while ((currentSeg == "HL") && (getSeg(currentLine, 3) == "23"))
            {
                Loop2000D();
            }
            if (currentSeg == "SE")
            {
                getNextSeg();
            } 
            else
            { 
                Console.WriteLine("Error missing SE"); //////////////////// ERROR
            }
            inLoopST = false;
        }

        public static void Loop2000A() //Information Source Level
        {
            inLoop2000A = true;
            getNextSeg();
            if (currentSeg == "AAA")
            {
                aaa();
                getNextSeg();
            }

            if (currentSeg == "NM1")
            {
                Loop2100A();
            }
            inLoop2000A = false;
        }

        public static void Loop2100A() //Information Source Name
        {
            inLoop2100A = true;
            currentSeg = getSeg(currentLine, 0);
            if (currentSeg == "NM1")
            {
                nm1();
                getNextSeg();
            }

            while (currentSeg == "REF")
            {
                reff();
                getNextSeg();
            }

            while (currentSeg == "PER")
            {
                per();
                getNextSeg();
            }

            while (currentSeg == "AAA")
            {
                aaa();
                getNextSeg();
            }
            inLoop2100A = false;
        }

        public static void Loop2000B() //Information Receiver Level
        {
            inLoop2000B = true;
            if (getSeg(currentLine, 3) == "21")
            {
                getNextSeg();

                if (currentSeg == "NM1")
                {
                    Loop2100B();
                }
            }
            inLoop2000B = false;
        }

        public static void Loop2100B()  //Information Receiver Name
        { 
            inLoop2100B = true;
            if (currentSeg == "NM1")
            {
                nm1();
                getNextSeg();
            }

            while (currentSeg == "REF")
            {
                reff();
                getNextSeg();
            }

            if (currentSeg == "N3")
            {
                n3();
                getNextSeg();
            }

            if (currentSeg == "N4")
            {
                n4();
                getNextSeg();
            }

            while (currentSeg == "AAA")
            {
                aaa();
                getNextSeg();
            }

            if (currentSeg == "PRV")
            {
                prv();
                getNextSeg();
            }
            inLoop2100B = false;
        }

        public static void Loop2000C() //Subscriber level
        { 
            inLoop2000C = true;
            if ((currentSeg == "HL") && (getSeg(currentLine, 3) == "22")) 
            {
                getNextSeg();

                while (currentSeg == "TRN")
                {
                    traceNumber = getSeg(currentLine, 2);
                    //addToBothCSVs(header,'TraceNumber',traceNumber); //////////////////// addToBothCSVs
                    getNextSeg();
                }

                if (currentSeg == "NM1")
                {
                    Loop2100C();
                }
            }
            inLoop2000C = false;
        }

        public static void Loop2100C()  //Subscriber Name
        { 
            inLoop2100C = true;
            if (getSeg(currentLine, 1) == "IL")
            {
                nm1();
                getNextSeg();
            }

            while (currentSeg == "REF")
            {
                reff();
                getNextSeg();
            }

            if (currentSeg == "N3")
            {
                n3();
                getNextSeg();
            }

            if (currentSeg == "N4")
            {
                n4();
                getNextSeg(); 
            }

            while (currentSeg == "PER")      //contact information        - no longer in 5010
            {
                per();
                getNextSeg();
            }

            while (currentSeg == "AAA")     //subscriber request validation
            {
                aaa();
                getNextSeg();
            }

            if (currentSeg == "PRV") 
            {
                prv();
                getNextSeg();
            }

            if (currentSeg == "DMG")     //Subscriber demographic information
            {
                dmg();
                getNextSeg();
            }

            if (currentSeg == "INS")     //Subscriber relationship
            {
                ins();
                getNextSeg();
            }

            if (currentSeg == "HI") 
            {
                hi();
                getNextSeg();
            }

            while (currentSeg == "DTP")     //Subscriber date
            {
                dtp();
                getNextSeg();
            }

            if (currentSeg == "MPI") 
            {
                mpi();
                getNextSeg();
            }

            while (currentSeg == "EB")
            {
                Loop2110C();
            }
            inLoop2100C = false;
        }

        public static void Loop2110C() //SUBSCRIBER ELIGIBILITY OR BENEFIT INFORMATION
        { 
            inLoop2110C = true;
            if (currentSeg == "EB") 
            {
                eb();
                getNextSeg();
            }

            while (currentSeg == "HSD")
            {
                hsd();
                getNextSeg();
            }

            while (currentSeg == "REF")
            {
                reff();
                getNextSeg();
            }

            while (currentSeg == "DTP")
            {
                dtp();
                getNextSeg();
            }

            while (currentSeg == "AAA")
            {
                aaa();
                getNextSeg();
            }
           
            while (currentSeg == "MSG")
            {
                msg();
                getNextSeg();
            }

            while (currentSeg == "III")
            {
                Loop2115C();
            }

            if (currentSeg == "LS") 
            {
                //this must have the value of "2120"
                if (getSeg(currentLine, 1) == "2120") 
                {
                 //output 2 spaces before the next NM101;   
                    Console.WriteLine("  "); //////////////////// write
                }
                getNextSeg();
            }

            if (currentSeg == "NM1") 
            {    
                Loop2120C();
            }

            if (currentSeg == "LE") 
            {    
                if (getSeg(currentLine, 1) == "2120")
                {
                    //great, do nothing;
                }
                getNextSeg();
            }

            inLoop2110C = false;
        }

        public static void Loop2115C() //SUBSCRIBER ELIGIBILITY OR BENEFIT ADDITIONAL INFORMATION
        {
            inLoop2115C = true;
            if (currentSeg == "III")
            {
                iii();
                getNextSeg();
            }
            inLoop2115C = false;
        }

        public static void Loop2120C() //SUBSCRIBER BENEFIT RELATED ENTITY NAME
        { 
            inLoop2120C = true;
            while (currentSeg == "NM1")
            {
                nm1();
                getNextSeg();
            }

            if (currentSeg == "N3") 
            {
                n3();
                getNextSeg();
            }

            if (currentSeg == "N4") 
            {
                n4();
                getNextSeg();
            }

            while (currentSeg == "PER") //contact information
            {
                per();
                getNextSeg();
            }

            if (currentSeg == "PRV") 
            {
                prv();
                getNextSeg();
            }

            inLoop2120C = false;
        }

        public static void Loop2000D() //Dependent level
        {
            inLoop2000D = true;
            if ((currentSeg == "HL") && (getSeg(currentLine, 3) == "23"))
            {
                getNextSeg();

                while (currentSeg == "TRN")
                {
                    dependentTaceNumber = getSeg(currentLine, 2);
                    Console.WriteLine("dependentTaceNumber: " + dependentTaceNumber);
                    getNextSeg();
                }

                if (currentSeg == "NM1")
                {
                    Loop2100D();
                }
            }
            inLoop2000D = false;
        }

        public static void Loop2100D()
        {
            inLoop2100D = true;
            if ((currentSeg == "NM1") && (getSeg(currentLine, 1) == "03"))
            {
                nm1();
                getNextSeg();
            }

            while (currentSeg == "REF")
            {
                reff();
                getNextSeg();
            }

            if (currentSeg == "N3")
            {
                n3();
                getNextSeg();
            }

            if (currentSeg == "N4")
            {
                n4();
                getNextSeg();
            }

            while (currentSeg == "PER") //contact information
            {
                per();
                getNextSeg();
            }

            while (currentSeg == "AAA") //dependent request validation
            {
                aaa();
                getNextSeg();
            }

            if (currentSeg == "PRV")
            {
                prv();
                getNextSeg();
            }

            if (currentSeg == "DMG") //dependent demographic information
            {
                dmg();
                getNextSeg();
            }

            if (currentSeg == "INS") //dependent relationship
            {
                ins();
                getNextSeg();
            }

            if (currentSeg == "HI")
            {
                hi();
                getNextSeg();
            }

            while (currentSeg == "DTP")     //dependent date
            {
                dtp();
                getNextSeg();
            }

            if (currentSeg == "MPI")
            {
                mpi();
                getNextSeg();
            }

            while (currentSeg == "EB")
            {
                Loop2110D();
            }
            inLoop2100D = false;
        }

        public static void Loop2110D() //Dependent eligibility or benefit information
        {
            inLoop2110D = true;
            if (currentSeg == "EB")
            {
                eb();
                getNextSeg();
            }

            while (currentSeg == "HSD")
            {
                hsd();
                getNextSeg();
            }

            while (currentSeg == "REF")
            {
                reff();
                getNextSeg();
            }

            while (currentSeg == "DTP")
            {
                dtp();
                getNextSeg();
            }

            while (currentSeg == "AAA")
            {
                aaa();
                getNextSeg();
            }

            while (currentSeg == "MSG")
            {
                msg();
                getNextSeg();
            }

            while (currentSeg == "III")
            {
                Loop2115D();
            }

            if (currentSeg == "LS")
            {
                if (getSeg(currentLine, 1) == "2120")
                { 
                    // great, do nothing
                }
                getNextSeg();
            }

            if (currentSeg == "NM1")
            {
                Loop2120D();
            }

            if (currentSeg == "LE")
            {
                if (getSeg(currentLine, 1) == "2120")
                { 
                    // great, do nothing
                }
                getNextSeg();
            }
            inLoop2110D = false;
        }

        public static void Loop2115D()  //DEPENDENT ELIGIBILITY OR BENEFIT ADDITIONAL
        {
            inLoop2115D = true;
            if (currentSeg == "III")
            {
                iii();
                getNextSeg();
            }
            inLoop2115D = false;
        }

        public static void Loop2120D() //DEPENDENT BENEFIT RELATED ENTITY NAME
        {
            inLoop2120D = true;
            while (currentSeg == "NM1")
            {
                nm1();
                getNextSeg();
            }

            if (currentSeg == "N3")
            {
                n3();
                getNextSeg();
            }

            if (currentSeg == "N4")
            {
                n4();
                getNextSeg();
            }

            while (currentSeg == "PER") //contact information
            {
                per();
                getNextSeg();
            }

            if (currentSeg == "PRV")
            {
                prv();
                getNextSeg();
            }
            inLoop2120D = false;
        }

        public static void nm1()
        {
            string nameLast = "";
            string nameFirst = "";
            string nameMiddle = "";
            string namePrefix = "";
            string nameSuffix = "";
            string IdCodeQualifier;
            string IDCode;
            string entityIdentifierCode = getIniValue("NM101", getSeg(currentLine, 1));
            string entityTypeQualifier = getSeg(currentLine, 2); // 1 = person, 2 = non-person

            elig271.objNm1 objNm1 = new elig271.objNm1();

            nameLast = getSeg(currentLine, 3);

            if (entityTypeQualifier == "1")
            {
                nameFirst = getSeg(currentLine, 4);
                nameMiddle = getSeg(currentLine, 5);
                namePrefix = getSeg(currentLine, 6);
                nameSuffix = getSeg(currentLine, 7);
            }

            if (nameSuffix != "")
            {
                nameLast = nameLast + " " + nameSuffix;
            }

            if (namePrefix != "")
            {
                nameFirst = nameSuffix + " " + nameFirst;
            }

            if (nameMiddle != "")
            {
                nameFirst = nameFirst + " " + nameMiddle;
            }

            if (nameFirst != "")
            {
                nameLast = nameLast + ", " + nameFirst;
            }

            IdCodeQualifier = getIniValue("NM108", getSeg(currentLine, 8));
            IDCode = getSeg(currentLine, 9);

            if (inLoop2100A)
            {
                //addToBothCSVs(header,'InsName',nameLast); //////////////////// addToBothCSVs
                InsName = nameLast;
            }
            else if (inLoop2100B)
            {
                // addToBothCSVs(header,'ProvTaxID',IDCode); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'ProvNameLast',nameLast); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'ProvNameFirst',nameFirst); //////////////////// addToBothCSVs
            }
            else if (inLoop2100C)
            {
                if (inLoop2120C)
                {
                    //addToBothCSVs(header,'PrimaryPayer',nameLast); //////////////////// addToBothCSVs
                }
                else
                {
                    // addToBothCSVs(header,'InsuredID',IDCode); //////////////////// addToBothCSVs
                    // addToBothCSVs(header,'InsuredNameLast',nameLast); //////////////////// addToBothCSVs
                    // addToBothCSVs(header,'InsuredNameFirst',nameFirst); //////////////////// addToBothCSVs
                    // addToBothCSVs(header,'InsuredNameMiddle',nameMiddle); //////////////////// addToBothCSVs
                }
            }
            else if (inLoop2100D)
            {
                // addToBothCSVs(header,'DependentID',IDCode); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'DependentNameLast',nameLast); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'DependentNameFirst',nameFirst); //////////////////// addToBothCSVs
            }

            //string lineToWrite = "";
            objNm1.lineToWrite = entityIdentifierCode + ": ";
            if (nameLast != "")
            {
                objNm1.lineToWrite += nameLast;
            }
            if (IdCodeQualifier != "")
            {
                objNm1.lineToWrite += " (" + IdCodeQualifier + ": " + IDCode + ")";
            }
            if (getSeg(lastEB, 3) == "42")  // what is lastEB?
            {
                objNm1.lineToWrite += "   ";
            }
            Console.WriteLine(objNm1.lineToWrite); //////////////////// writeln OBJ
        }

        public static void per()
        {
            elig271.objPer objPer = new elig271.objPer();
            
            string contactName;
            //string communicationNumberQual;
            string communicationNumber;
            //string communicationNumberQual2;
            string communicationNumber2;
            //string communicationNumberQual3;
            string communicationNumber3;

            contactName = getSeg(currentLine, 2);
            objPer.communicationNumberQual = getIniValue("PER03", getSeg(currentLine, 3)); //check PER03.txt
            communicationNumber = getSeg(currentLine, 4);
            objPer.communicationNumberQual2 = getIniValue("PER03", getSeg(currentLine, 5)); //check PER03.txt
            communicationNumber2 = getSeg(currentLine, 6);
            objPer.communicationNumberQual3 = getIniValue("PER03", getSeg(currentLine, 7)); //check PER03.txt
            communicationNumber3 = getSeg(currentLine, 8);

            if (contactName != "")
            {
                Console.WriteLine("   " + contactName); //////////////////// writeln
            }

            if (objPer.communicationNumberQual != "")
            {
                objPer.communicationNumberQual = objPer.communicationNumberQual + ": " + communicationNumber;
                Console.WriteLine("   " + objPer.communicationNumberQual); //////////////////// writeln
            }

            if (objPer.communicationNumberQual2 != "")
            {
                objPer.communicationNumberQual2 = objPer.communicationNumberQual2 + ": " + communicationNumber2;
                Console.WriteLine("   " + objPer.communicationNumberQual2); //////////////////// writeln
            }

            if (objPer.communicationNumberQual3 != "")
            {
                objPer.communicationNumberQual3 = objPer.communicationNumberQual3 + ": " + communicationNumber3;
                Console.WriteLine("   " + objPer.communicationNumberQual3); //////////////////// writeln
            }

            if (inLoop2120C)
            {
                //addToBothCSVs(header,'PrimPayPhone',communicationNumber); //////////////////// addToBothCSVs
            }
        }

        public static void n3()
        {
            elig271.objN3 objN3 = new elig271.objN3();
            objN3.address1 = getSeg(currentLine, 1);
            objN3.address2 = getSeg(currentLine, 2);

            if (objN3.address1 != "")
            {
                Console.WriteLine("     " + objN3.address1); //////////////////// writeln OBJ
            }

            if (objN3.address2 != "")
            {
                Console.WriteLine("     " + objN3.address2); //////////////////// writeln OBJ
            }

            if (inLoop2100C)
            {
                // addToBothCSVs(header,'PrimaryPayerStreet',address1); //////////////////// addToBothCSVs
            }
            else
            {
                // addToBothCSVs(header,'InsuredSreet',address1); //////////////////// addToBothCSVs
            }
        }

        public static void n4()
        {
            elig271.objN4 objN4 = new elig271.objN4();
            //string cityStateZip = "";

            if ((getSeg(currentLine, 1) != "") || (getSeg(currentLine, 2) != "") || (getSeg(currentLine, 3) != ""))
            {
                objN4.cityStateZip = getSeg(currentLine, 1) + ", " + getSeg(currentLine, 2) + ", " + getSeg(currentLine, 3);
            }

            if (getSeg(currentLine, 4) != "")
            {
                objN4.cityStateZip += " " + getSeg(currentLine, 4);
            }

            if ((getSeg(currentLine, 5) != "") && (getSeg(currentLine, 6) != ""))
            {
                objN4.cityStateZip += " " + getIniValue("N405", getSeg(currentLine, 5)) + ": " + getSeg(currentLine, 6);
            }

            if (inLoop2120C)
            {
                // addToBothCSVs(header,'PrimaryPayerCity',get(s,1)); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'PrimaryPayerState',get(s,2)); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'PrimaryPayerZip',get(s,3)); //////////////////// addToBothCSVs
            }
            else
            {
                // addToBothCSVs(header,'InsuredCity',get(s,1)); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'InsuredState',get(s,2)); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'InsuredZip',get(s,3)); //////////////////// addToBothCSVs
            }

            Console.WriteLine("     " + objN4.cityStateZip); //////////////////// writeln OBJ
        }

        public static void reff()
        {
            elig271.objReff objReff = new elig271.objReff();

            string PrimPaycontractNumber;
            string PrimPayplanNumber = "";
            string refIDQual = getIniValue("REF01", getSeg(currentLine, 1));       //check REF01.txt
            string refID = getSeg(currentLine, 2);
            objReff.refName = getSeg(currentLine, 3);

            if (refIDQual != "Submitter Identification Number")
            {
                objReff.refWrite = refIDQual + ": " + refID;
                Console.Write("    " + objReff.refWrite); //////////////////// write OBJ
            }

            if (objReff.refName != "")
            {
                objReff.refName = "(" + objReff.refName + ")";
                Console.WriteLine(objReff.refName); //////////////////// writeln OBJ
            }
            else
            {
                Console.WriteLine(""); //////////////////// writeln
            }

            if (inLoop2120C)
            {
                if (refID.IndexOf(" ") != 0) // make sure this if is correct
                {
                    PrimPaycontractNumber = refID.Substring(1, refID.IndexOf(' ') - 1);
                    PrimPayplanNumber = refID.Substring(refID.IndexOf(' ') + 1, 25);
                }
                else
                {
                    PrimPaycontractNumber = refID;
                }

                // addToBothCSVs(header,'PrimaryPayerContractNumber', PrimPaycontractNumber); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'PrimaryPayerPlanNumber', PrimPayplanNumber); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'PrimaryPayerIDType',refIDQual); //////////////////// addToBothCSVs
            }
        }

        public static void hi()
        {
            // do nothing with this for now
        }

        public static void mpi()
        {
            // do nothing with this for now
        }

        public static void ins()
        {
            elig271.objIns objIns = new elig271.objIns();

            if ((getSeg(currentLine, 3) == "001") && (getSeg(currentLine, 4) == "25"))
            {
                //Use this code to indicate that a change has been made to the primary elements that identify a specific person. Such elements are first name, last name, date of birth, identification numbers, and address.
                objIns.insNote = "Note: Subscriber information has changed from the information you submitted";
                Console.WriteLine(objIns.insNote); //////////////////// writeln OBJ
            }

            objIns.studentStatusCode = getIniValue("INS09", getSeg(currentLine, 9));
            objIns.handicapIndicator = getSeg(currentLine, 10);
            objIns.birthSequenceNumber = getSeg(currentLine, 17); // INS17 is the number assigned to each family member born with the same birth date. This number identifies birth sequence for multiple births allowing proper tracking and response of benefits for each dependent (i.e., twins, triplets, etc.).
            if (objIns.studentStatusCode != "")
            {
                objIns.studentStatusCode = "Student: " + objIns.studentStatusCode;
                Console.WriteLine(objIns.studentStatusCode); //////////////////// writeln OBJ
            }

            if (objIns.handicapIndicator != "")
            {
                objIns.handicapIndicator = "Handicap: " + objIns.handicapIndicator;
                Console.WriteLine(objIns.handicapIndicator); //////////////////// writeln OBJ
            }

            if (objIns.birthSequenceNumber != "")
            {
                objIns.birthSequenceNumber = "Birth Sequence Number: " + objIns.birthSequenceNumber;
                Console.WriteLine(objIns.birthSequenceNumber); //////////////////// writeln OBJ
            }

        }

        public static void dmg()
        {
            elig271.objDmg objDmg = new elig271.objDmg();
            //string birthDate;
            //string gender;

            objDmg.birthDate = outputDate(getSeg(currentLine, 2));
            objDmg.gender = getSeg(currentLine, 3);

            if (objDmg.birthDate != "")
            {
                objDmg.birthDate = "DOB: " + objDmg.birthDate;
            }
            if (objDmg.gender != "")
            {
                objDmg.gender = "Gender: " + objDmg.gender;
            }
            Console.WriteLine(objDmg.birthDate); //////////////////// writeln OBJ
            Console.WriteLine(objDmg.gender); //////////////////// writeln OBJ

            // addToBothCSVs(header,'InsuredDOB',birthDate); //////////////////// addToBothCSVs
            // addToBothCSVs(header,'InsuredSex',gender); //////////////////// addToBothCSVs
        }

        public static void dtp()
        {
            elig271.objDtp objDtp = new elig271.objDtp();

            string dateTimePeriod = "";
            string dateTimeQual = getIniValue("DTP01", getSeg(currentLine, 1));  //check DTP01.txt
            if (getSeg(currentLine, 2) == "RD8")
            {
                //date is CCYYMMDD-CCYYMMDD
                dateTimePeriod = outputDate(getSeg(currentLine, 3));
            }
            else if (getSeg(currentLine, 2) == "D8")
            {
                //date is CCYYMMDD
                dateTimePeriod = outputDate(getSeg(currentLine, 3));
            }

            objDtp.dateTimeTotal = dateTimeQual + ": " + dateTimePeriod;

            Console.WriteLine("     " + objDtp.dateTimeTotal); //////////////////// writeln OBJ

            if (inLoop2120C)
            {
                // addToBothCSVs(header,'PrimaryPayerDates',dateTimePeriod); //////////////////// addToBothCSVs
            }
            else if (inLoop2110C)
            {
                // addToBothCSVs(header,'EBSubsDateQual',dateTimePeriod); //////////////////// addToBothCSVs
            }
        }

        public static void prv()
        {
            elig271.objPrv objPrv = new elig271.objPrv();

            string providerCode = getIniValue("PRV01", getSeg(currentLine, 1)); //prv01
            string referenceIDQualifier = getIniValue("PRV02",getSeg(currentLine, 2));     //prv02
            string providerID = getSeg(currentLine, 3);
            objPrv.prvWrite = providerCode + " " + referenceIDQualifier + ": " + providerID;
            Console.WriteLine("     " + objPrv.prvWrite); //////////////////// writeln OBJ
        }

        public static void aaa()
        {
            string validRequestIndicator = getSeg(currentLine, 1);
            string rejectReasonCode = getSeg(currentLine, 3);
            string followUpActionCode = getSeg(currentLine, 4);
            //string validRequestIndicatorY;
            //string validRequestIndicatorN;
            //string rejectReason;
            //string followUpAction;

            elig271.objAaa objAaa = new elig271.objAaa();

            Console.WriteLine("");  //////////////////// writeln

            if (validRequestIndicator == "Y")
            {
                objAaa.validRequestIndicatorY = "This request was valid, however the transaction was rejected due to the following reason:";
                //objAaa.validRequestIndicatorY = validRequestIndicatorY;
                Console.WriteLine(objAaa.validRequestIndicatorY); //////////////////// writeln OBJ
            }
            else
            {
                objAaa.validRequestIndicatorN = "This request or an element in this request was NOT valid. This transaction was rejected due to the following reason:";
                //objAaa.validRequestIndicatorN = validRequestIndicatorN;
                Console.WriteLine(objAaa.validRequestIndicatorN); //////////////////// writeln OBJ
            }
            objAaa.rejectReason = getIniValue("AAA03", rejectReasonCode);
            objAaa.followUpAction = getIniValue("AAA04", followUpActionCode);
            Console.WriteLine(objAaa.rejectReasonCode + ": " + objAaa.rejectReason); //////////////////// writeln OBJ
            Console.WriteLine(objAaa.followUpAction); //////////////////// writeln OBJ
            Console.WriteLine(""); //////////////////// writeln

            // addToBothCSVs(header,'Error',rejectReasonCode+': '+getIniValue('AAA03',rejectReasonCode)); //////////////////// addToBothCSVs
        }

        public static void iii()
        {
            elig271.objIii objIii = new elig271.objIii();

            string industryCode;
            string codeListQualifierCode = getIniValue("III01", getSeg(currentLine, 1));
            if (getSeg(currentLine, 1) == "ZZ")
            {
                industryCode = getIniValue("III02", getSeg(currentLine, 2));
            }
            else
            {
                industryCode = getSeg(currentLine, 2);
            }

            objIii.iiiWrite = codeListQualifierCode + ": " + industryCode;
            Console.WriteLine("     " + objIii.iiiWrite); //////////////////// writeln OBJ

            // addToBothCSVs(header,'QualCode',codeListQualifierCode); //////////////////// addToBothCSVs
            // addToBothCSVs(header,'Code', industryCode); //////////////////// addToBothCSVs
        }
       
        public static void eb()
        {
            elig271.objEb objEb = new elig271.objEb();

            ebCount++;
            List<string> serviceTypeCodes = new List<string>();
            //string eligibilityInfo;
            //string coverageLevelCode;
            string serviceTypeCode;
            //string insuranceTypeCode;
            string planCoverageDesc;
            //string timePeriodQual;
            //string benefitAmount;
            //string benefitPercent;
            //string quantityQualifier;
            string benefitQuantity;
            //string authOrCertIndicator;
            string inPlanNetworkIndicator;
            string productServiceIDQual;
            string procedureCode;
            //string procedureModifier;
            objEb.diagPointer = "";
            //string serviceTypeCodeWrite;

            objEb.eligibilityInfo = getIniValue("EB01", getSeg(currentLine, 1));
            objEb.coverageLevelCode = getIniValue("EB02", getSeg(currentLine, 2));
            int i = 1;
            while (getsub(currentLine, repetitionDelim, 3, i) != "")
            {
               serviceTypeCodes.Add(getIniValue("EB03", getsub(currentLine, repetitionDelim, 3, i)));
               i++;
            }
            serviceTypeCode = getIniValue("EB03", getSeg(currentLine, 3));
            objEb.insuranceTypeCode = getIniValue("EB04", getSeg(currentLine, 4));
            planCoverageDesc = getSeg(currentLine, 5);
            objEb.timePeriodQual = getIniValue("EB06", getSeg(currentLine, 6));
            //this is for Medicare - they are returning 29 - Remaining for AD, AE, and AF, so I have to chnage it to Used
            if ((InsName == "CMS") && (objEb.timePeriodQual == "Remaining") && ((getSeg(currentLine, 3) == "AD") || (getSeg(currentLine, 3) == "AE") || (getSeg(currentLine, 3) == "AF")))
            {
                objEb.timePeriodQual = "Used";
            }

            objEb.benefitAmount = getSeg(currentLine, 7);
            objEb.benefitPercent = getSeg(currentLine, 8);
            objEb.quantityQualifier = getIniValue("EB09", getSeg(currentLine, 9));
            benefitQuantity = getSeg(currentLine, 10);
            objEb.authOrCertIndicator = getSeg(currentLine, 11);
            inPlanNetworkIndicator = getSeg(currentLine, 12);
            productServiceIDQual = getIniValue("EB13-1", getsub(currentLine, subElementDelim, 13, 1));
            procedureCode = getsub(currentLine, subElementDelim, 13, 2);
            objEb.procedureModifier = getsub(currentLine, subElementDelim, 13, 3);

            for (i = 4; i <= 6; i++)
            {
                if (getsub(currentLine, subElementDelim, 13, i) != "")
                    objEb.procedureModifier = objEb.procedureModifier + "," + getsub(currentLine, subElementDelim, 13, i);
            }
            for (i = 1; i <= 4; i++)
            {
                if (getsub(currentLine, subElementDelim, 14, i) != "")
                {
                    objEb.diagPointer += "," + getsub(currentLine, subElementDelim, 14, i);
                }
            }
            
            if (getIniValue("EB03", getSeg(lastEB, 3)) != serviceTypeCode)
            {
                Console.WriteLine(""); //////////////////// writeln
                for (i = 0; i < serviceTypeCodes.Count - 1; i++) 
                {
                    objEb.serviceTypeCodeWrite = serviceTypeCodes[i] + " - " + getsub(currentLine, repetitionDelim, 3, i + 1);
                    Console.WriteLine(objEb.serviceTypeCodeWrite); //////////////////// writeln OBJ
                }
                lastEB = "";
            }

            if (getIniValue("EB01", getSeg(lastEB, 1)) != objEb.eligibilityInfo)
            {
                if (objEb.eligibilityInfo.ToUpper().IndexOf("ACTIVE") != 0)
                {
                    objEb.eligibilityInfo = "Status: " + objEb.eligibilityInfo;
                    Console.WriteLine(objEb.eligibilityInfo); //////////////////// writeln OBJ
                }
                else
                {
                    Console.WriteLine(" " + objEb.eligibilityInfo); //////////////////// writeln OBJ
                }
                lastEB = "EB*" + objEb.eligibilityInfo;
            }

            if ((getIniValue("EB02", getSeg(lastEB, 2)) != objEb.coverageLevelCode) && (objEb.coverageLevelCode != ""))
            {
                objEb.coverageLevelCode = "Coverage Level: " + objEb.coverageLevelCode;
                Console.WriteLine(" " + objEb.coverageLevelCode); //////////////////// writeln OBJ
                lastEB = "EB*" + objEb.eligibilityInfo;
            }

            if (((getIniValue("EB05", getSeg(lastEB, 5)) != planCoverageDesc) && (planCoverageDesc != "")) || ((getIniValue("EB04", getSeg(lastEB, 4)) != objEb.insuranceTypeCode) && (objEb.insuranceTypeCode != "")))
            {
                if (planCoverageDesc != "")
                {
                    objEb.insuranceTypeCode = objEb.insuranceTypeCode + " (" + planCoverageDesc + ")";
                }
                Console.WriteLine("  " + objEb.insuranceTypeCode); //////////////////// writeln OBJ
                lastEB = "EB*" + objEb.eligibilityInfo + "*" + objEb.coverageLevelCode;
            }

            if (objEb.benefitAmount != "")
            {
                objEb.benefitAmount = "Amount: $" + objEb.benefitAmount;
                Console.Write(objEb.benefitAmount); //////////////////// write OBJ
            }
            if (objEb.benefitPercent != "") 
            {
                objEb.benefitPercent = "Percent: " + float.Parse(objEb.benefitPercent) * 100 + "%";
                Console.Write(objEb.benefitPercent); //////////////////// write OBJ
            }
            if ((objEb.benefitAmount != "") || (objEb.benefitPercent != ""))
            {
                if (objEb.timePeriodQual == "Remaining")
                {
                    Console.Write(" " + objEb.timePeriodQual); //////////////////// write OBJ
                }
                else if (objEb.timePeriodQual != "")
                {
                    Console.Write("  per " + objEb.timePeriodQual); //////////////////// write OBJ
                }
                Console.WriteLine(""); //////////////////// writeln
            }
            else
            {
                if (objEb.timePeriodQual != "")
                {
                    Console.WriteLine("    " + objEb.timePeriodQual); //////////////////// writeln OBJ
                }
            }

            if ((benefitQuantity != "") && (objEb.quantityQualifier != ""))
            {
                objEb.benefitQuanQual = benefitQuantity + " " + objEb.quantityQualifier;
                Console.WriteLine("      " + objEb.benefitQuanQual); //////////////////// writeln OBJ
            }
            else if (objEb.quantityQualifier != "") 
            {
                Console.WriteLine("      " + objEb.quantityQualifier); //////////////////// writeln OBJ
            }
            if (objEb.authOrCertIndicator != "")
            {
                objEb.authOrCertIndicator = "Authorization or Certificate Required: " + objEb.authOrCertIndicator;
                Console.WriteLine("       " + objEb.authOrCertIndicator); //////////////////// writeln OBJ
            }
            if (inPlanNetworkIndicator == "Y")
            {
                objEb.inPlanNetworkIndicatorY = "Benefits are In-Plan-Network";
                Console.WriteLine("       " + objEb.inPlanNetworkIndicatorY); //////////////////// writeln OBJ
            }
            else if (inPlanNetworkIndicator == "N")
            {
                objEb.inPlanNetworkIndicatorN = "Benefits are Out-Of-Plan-Network";
                Console.WriteLine("       " + objEb.inPlanNetworkIndicatorN); //////////////////// writeln OBJ
            }
            if (productServiceIDQual != "")
            {
                if (procedureCode == "G0180")
                {
                    objEb.G0180 = "Home Health Certification (G0180)";
                    Console.WriteLine("    " + objEb.G0180); //////////////////// writeln OBJ
                }
                else if (procedureCode == "G0179")
                {
                    objEb.G0179 = "Home Health Recertification (G0179)";
                    Console.WriteLine("   " + objEb.G0179); //////////////////// writeln OBJ
                }
                else
                {
                    objEb.prodProc = productServiceIDQual + ": " + procedureCode;
                    Console.WriteLine("       " + objEb.prodProc); //////////////////// writeln OBJ
                }

            }

            if (objEb.procedureModifier != "")
            {
                objEb.procedureModifier = "Modifiers: " + objEb.procedureModifier;
                Console.WriteLine("        " + objEb.procedureModifier); //////////////////// writeln OBJ
            }
            if (objEb.diagPointer != "")
            {
                objEb.diagPointer = "Diag Pointers: " + objEb.diagPointer;
                Console.WriteLine("        " + objEb.diagPointer); //////////////////// writeln OBJ
            }
            lastEB = currentLine;

            if (inLoop2120C)
            {
                // addToBothCSVs(header,'PrimaryPayerType',insuranceTypeCode); //////////////////// addToBothCSVs
            }
            else
            {
                // addToBothCSVs(header,'EligInfo'+inttostr(ebCount),eligibilityInfo); //////////////////// addToBothCSVs
                // addToBothCSVs(header,'CoveLeve'+inttostr(ebCount), coverageLevelCode); //////////////////// addToBothCSVs

                for(i = 0; i < serviceTypeCodes.Count(); i++)
                {
                    // addToBothCSVs(header,'ServTypeCode'+inttostr(ebCount)+inttostr(i),serviceTypeCodes[i]); //////////////////// addToBothCSVs
                }

                //addToBothCSVs(header,'InsuTypeCode'+inttostr(ebCount),insuranceTypeCode); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'PlanCoveDesc'+inttostr(ebCount),planCoverageDesc); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'TimePeriQual'+inttostr(ebCount), timePeriodQual); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'MoneAmou'+inttostr(ebCount), benefitAmount); //////////////////// addToBothCSVs

                if (objEb.benefitPercent != "")
                {
                    //addToBothCSVs(header,'Perc'+inttostr(ebCount), floattostr(strtofloat(benefitPercent)*100)+'%')  //////////////////// addToBothCSVs
                }
                else
                {
                    //addToBothCSVs(header,'Perc'+inttostr(ebCount), ''); //////////////////// addToBothCSVs
                }
                //addToBothCSVs(header,'OtheQuanQual'+inttostr(ebCount), quantityQualifier); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'OtheQuan'+inttostr(ebCount), benefitQuantity); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'AuthRequ'+inttostr(ebCount), authOrCertIndicator); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'BeneCons'+inttostr(ebCount), inPlanNetworkIndicator); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'ProcCodeQual'+inttostr(ebCount), productServiceIDQual); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'ProcCode'+inttostr(ebCount), procedureCode); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'Mod1'+inttostr(ebCount), procedureModifier); //////////////////// addToBothCSVs
                //addToBothCSVs(header,'DiagPointer'+inttostr(ebCount), diagPointer); //////////////////// addToBothCSVs
            }
        }

        public static void hsd()
        {
            string quantity = "";
            string quantity2 = "";
            string quantity3 = "";
            string quantity4 = "";
            string quantityQualifier = getIniValue("HSD01", getSeg(currentLine, 1));
            string benefitQuantity = getSeg(currentLine, 2);
            string unitOrBasisForMeasurementCode = getIniValue("HSD03", getSeg(currentLine, 3));
            string sampleSelectionModulus = getSeg(currentLine, 4);
            string timePeriodQualifier = getIniValue("HSD05", getSeg(currentLine, 5));
            string periodCount = getSeg(currentLine, 6);
            string deliveryFrequencyCode = getIniValue("HSD07", getSeg(currentLine, 7));
            string deliveryPatternTimeCode = getIniValue("HSD08", getSeg(currentLine, 8));

            elig271.objHsd objHsd = new elig271.objHsd();

            quantity = benefitQuantity + " " + quantityQualifier;
            if (sampleSelectionModulus != "")
            {
                quantity2 = sampleSelectionModulus + " " + quantityQualifier + " per " + unitOrBasisForMeasurementCode;

                if (timePeriodQualifier != "")
                {
                    if (periodCount != "")
                    {
                        quantity3 = ", for " + periodCount + " " + timePeriodQualifier;
                    }
                    else
                    {
                        quantity3 = " per " + timePeriodQualifier;
                    }
                }
            }
            else
            {
                if (timePeriodQualifier != "")
                {
                    if ((timePeriodQualifier == "Exceeded") && (unitOrBasisForMeasurementCode == "Days"))
                    {
                        quantity3 = " From Day " + (Int32.Parse(periodCount)+1).ToString();
                    }
                    else if ((timePeriodQualifier == "Not Exceeded") && (unitOrBasisForMeasurementCode == "Days"))
                    {
                        quantity3 = " Thru Day " + periodCount;
                    }
                    else if ((periodCount != "") && (unitOrBasisForMeasurementCode != ""))
                    {
                        quantity3 = " for " + periodCount + " " + unitOrBasisForMeasurementCode + " " +timePeriodQualifier;
                    }
                    else if (periodCount != "")
                    {
                        quantity3 = " for " + periodCount + " " + timePeriodQualifier;
                    }
                    else
                    {
                        quantity3 = " per " + timePeriodQualifier;
                    }
                }
            }

            quantity4 = " " + deliveryFrequencyCode + " " + deliveryPatternTimeCode;
            objHsd.quant = quantity + quantity2 + quantity3 + quantity4;
            Console.WriteLine("     " + objHsd.quant); //////////////////// writeln OBJ

            //addToBothCSVs(header,'QuanQual',quantityQualifier); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'Quan',benefitQuantity); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'BasisMeasCode',unitOrBasisForMeasurementCode); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'FurtherInfo', sampleSelectionModulus); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'TimePeriodQualifier2',timePeriodQualifier); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'NumberOfPeriods',periodCount); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'DeliFreqCode',deliveryFrequencyCode); //////////////////// addToBothCSVs
            //addToBothCSVs(header,'DeliPattTimeCode',deliveryPatternTimeCode); //////////////////// addToBothCSVs
           
        }

        public static void msg()
        {
            elig271.objMsg objMsg = new elig271.objMsg();

            objMsg.freeFormMessageText = getSeg(currentLine, 1);
            Console.WriteLine("     " + objMsg.freeFormMessageText); //////////////////// writeln OBJ

            //addToBothCSVs(header,'EBMessText',freeformMessageText); //////////////////// addToBothCSVs
        }

        public static string outputDate(string currentLine)
        {
            if (currentLine.IndexOf("-") != -1)
            {
                return outputDate(currentLine.Substring(0, 8)) + " - " + outputDate(currentLine.Substring(9, 8));
            }
            else
            {
                return currentLine.Substring(4, 2) + "/" + currentLine.Substring(6, 2) + "/" + currentLine.Substring(0, 4);
            }
        }

        //get Ini value -- make this work 
        public static string getIniValue(string section, string key)
        {
            // ready ini file, find the section use the key to return the value
            string basePath = @"C:\home\elig\";
            IniFile ini = new IniFile(Path.Combine(basePath, "AllCodes-Emdeon.ini"));
            string retVal = string.Empty;

            if (key != "")
            {
                retVal = ini.IniReadValue(section, key);

                //result := iniFile.ReadString(section,key,'Code Missing - Contact Cortex ('+section+key+')');
                //result := StringReplace(result,'@',#13#10,[rfReplaceAll]);

                //Console.WriteLine(retVal);
                return retVal;
            }

            else
            {
                //Console.WriteLine("");
                return "";
            }

        }
        
        // gets segment of line
        public static string getSeg(string currentLine, int index)
        {
            index++;
            //string[] sString = s.Split(elementDelim);
            //return sString[index].Trim();            
            if ((currentLine != "") && (currentLine[currentLine.Length - 1] == segmentDelim))
            {
                currentLine = currentLine.TrimEnd(segmentDelim);
            }
            return GetVal("|" + elementDelim + "|" + currentLine, index);
        }

        // gets next line
        public static string getNextLine()
        {
            return currentLine = file.ReadLine().TrimEnd(segmentDelim); ;
        }

        // if string isnt empty, and the last char of string == segmentDelim then delete it 
        // return the segment (get val)
        public static string getsub(string currentLine, char delim, int index)
        {
            // inc(index);
            if ((currentLine != "") && (currentLine[currentLine.Length - 1 ] == segmentDelim))
            {
                currentLine = getNextLine();
                currentLine = currentLine.TrimEnd(segmentDelim);
            }
            return GetVal("|" + delim + "|" + currentLine, index);
        }

        // element within and element 
        public static string getsub(string currentLine, char delim, int index1, int index2)
        {
            string elem;
            // inc(index);
            elem = getSeg(currentLine, index1);
            return getsub(elem, delim, index2);
        }

        // see if you need to use this - Not 0 based
        public static string GetVal(string s2, int n)
        {
            string d = ",";

            if ((s2.Length >= 2) && (s2[0] == '|') && (s2[2] == '|')) 
            {
              d = Char.ToString(s2[1]);
              s2 = s2.Remove(0, 3);
            }
            
            s2 = d + s2 + d;
            string result = string.Empty;
            for (int i = 1; i <= n; i++)
            {
                s2 = s2.Remove(0, 1);
                bool q = s2[0] == '"';
                string s3 = s2;
                string s;
                if (q)
                {
                    s2 = s2.Remove(0, 1);
                    s3 = s2;
                    s2 = s2.Replace("\"\"", "¾¾");
                    s = '"' + d;
                }
                else
                    s = d;

                int a = s2.IndexOf(s);
                if (a == -1) result = string.Empty;
                else result = s2.Substring(0, a);
                s2 = s3;
                s2 = s2.Remove(0, a);
                if (q) s2 = s2.Remove(0, 1);
                s2 = s2 + d;
            }
            result = result.Replace("¾¾", "\"");
            return result.Trim();
        }
    }
}

