using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eligRequest
{
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

                //System.Console.WriteLine(currentLine);

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

                ////////////////////////shouldnt i get the next line here??
                getNextSeg();

                if (currentSeg != "IEA") // IEA is the closing segment to ISA
                {
                    // error missing IEA
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
                Console.WriteLine("                        ELIGIBILITY - " + outputDate(DateTranCrea)); //////////////////// writeln
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
                //////////////////////// why do nothing here? dont i need to get the next line?
                getNextSeg();
                ///////////////////////////
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

            // getIniValue("NM101", "PR"); -- get this to work later
            string entityIdentifierCode = getIniValue("NM101", getSeg(currentLine, 1));
            string entityTypeQualifier = getSeg(currentLine, 2); // 1 = person, 2 = non-person
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

            // line to write
            string lineToWrite = "";
            lineToWrite = entityIdentifierCode + ": ";
            if (nameLast != "")
            {
                lineToWrite += nameLast;
            }
            if (IdCodeQualifier != "")
            {
                lineToWrite += " (" + IdCodeQualifier + ": " + IDCode + ")";
            }
            if (getSeg(lastEB, 3) == "42")  // what is lastEB?
            {
                lineToWrite += "   ";
            }
            Console.WriteLine(lineToWrite); //////////////////// writeln
        }

        public static void per()
        {
            string contactName;
            string communicationNumberQual;
            string communicationNumber;
            string communicationNumberQual2;
            string communicationNumber2;
            string communicationNumberQual3;
            string communicationNumber3;

            contactName = getSeg(currentLine, 2);
            communicationNumberQual = getIniValue("PER03", getSeg(currentLine, 3)); //check PER03.txt
            communicationNumber = getSeg(currentLine, 4);
            communicationNumberQual2 = getIniValue("PER03",getSeg(currentLine, 5)); //check PER03.txt
            communicationNumber2 = getSeg(currentLine, 6);
            communicationNumberQual3 = getIniValue("PER03", getSeg(currentLine, 7)); //check PER03.txt
            communicationNumber3 = getSeg(currentLine, 8);

            if (contactName != "")
            {
                Console.WriteLine("   " + contactName); //////////////////// writeln
            }

            if (communicationNumberQual != "")
            {
                Console.WriteLine("   " + communicationNumberQual + ": " + communicationNumber); //////////////////// writeln
            }

            if (communicationNumberQual2 != "")
            {
                Console.WriteLine("   " + communicationNumberQual2 + ": " + communicationNumber2); //////////////////// writeln
            }

            if (communicationNumberQual3 != "")
            {
                Console.WriteLine("   " + communicationNumberQual3 + ": " + communicationNumber3); //////////////////// writeln
            }

            if (inLoop2120C)
            {
                //addToBothCSVs(header,'PrimPayPhone',communicationNumber); //////////////////// addToBothCSVs
            }
        }

        public static void n3()
        {
            string address1 = getSeg(currentLine, 1);
            string address2 = getSeg(currentLine, 2);

            if (address1 != "")
            {
                Console.WriteLine("     " + address1); //////////////////// writeln
            }

            if (address2 != "")
            {
                Console.WriteLine("     " + address2); //////////////////// writeln
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
            string cityStateZip = "";

            if ((getSeg(currentLine, 1) != "") || (getSeg(currentLine, 2) != "") || (getSeg(currentLine, 3) != ""))
            {
                cityStateZip = getSeg(currentLine, 1) + ", " + getSeg(currentLine, 2) + ", " + getSeg(currentLine, 3);
            }

            if (getSeg(currentLine, 4) != "")
            {
                cityStateZip += " " + getSeg(currentLine, 4);
            }

            if ((getSeg(currentLine, 5) != "") && (getSeg(currentLine, 6) != ""))
            {
                cityStateZip += " " + getIniValue("N405", getSeg(currentLine, 5)) + ": " + getSeg(currentLine, 6);
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

            Console.WriteLine("     " + cityStateZip); //////////////////// writeln
        }

        public static void reff()
        {
            string PrimPaycontractNumber;
            string PrimPayplanNumber = "";
            string refIDQual = getIniValue("REF01", getSeg(currentLine, 1));       //check REF01.txt
            string refID = getSeg(currentLine, 2);
            string refName = getSeg(currentLine, 3);

            if (refIDQual != "Submitter Identification Number")
            {
                Console.Write("    " + refIDQual + ": " + refID); //////////////////// write
            }

            if (refName != "")
            {
                Console.WriteLine(" (" + refName + ")"); //////////////////// writeln
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
            if ((getSeg(currentLine, 3) == "001") && (getSeg(currentLine, 4) == "25"))
            {
                //Use this code to indicate that a change has been made to the primary elements that identify a specific person. Such elements are first name, last name, date of birth, identification numbers, and address.
                Console.WriteLine("Note: Subscriber information has changed from the information you submitted"); //////////////////// writeln
            }

            string studentStatusCode = getIniValue("INS09", getSeg(currentLine, 9));
            string handicapIndicator = getSeg(currentLine, 10);
            string birthSequenceNumber = getSeg(currentLine, 17); // INS17 is the number assigned to each family member born with the same birth date. This number identifies birth sequence for multiple births allowing proper tracking and response of benefits for each dependent (i.e., twins, triplets, etc.).
            if (studentStatusCode != "")
            {
                Console.WriteLine("Student: " + studentStatusCode); //////////////////// writeln
            }

            if (handicapIndicator != "")
            {
                Console.WriteLine("Handicap: " + handicapIndicator); //////////////////// writeln
            }

            if (birthSequenceNumber != "")
            {
                Console.WriteLine("Birth Sequence Number: " + birthSequenceNumber); //////////////////// writeln
            }

        }

        public static void dmg()
        {
            string birthDate;
            string gender;
            string linetowrite = "";

            birthDate = outputDate(getSeg(currentLine, 2));
            gender = getSeg(currentLine, 3);

            if (birthDate != "")
            {
                linetowrite += "DOB: " + birthDate;
            }
            if (gender != "")
            {
                linetowrite += " Gender: " + gender;
            }
            Console.WriteLine(linetowrite); //////////////////// writeln

            // addToBothCSVs(header,'InsuredDOB',birthDate); //////////////////// addToBothCSVs
            // addToBothCSVs(header,'InsuredSex',gender); //////////////////// addToBothCSVs
        }

        public static void dtp()
        {
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

            Console.WriteLine("     " + dateTimeQual + ": " + dateTimePeriod); //////////////////// writeln

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
            string providerCode = getIniValue("PRV01", getSeg(currentLine, 1)); //prv01
            string referenceIDQualifier = getIniValue("PRV02",getSeg(currentLine, 2));     //prv02
            string providerID = getSeg(currentLine, 3);
            Console.WriteLine("     " + providerCode + " " + referenceIDQualifier + ": " + providerID); //////////////////// writeln
        }

        public static void aaa()
        {
            string validRequestIndicator = getSeg(currentLine, 1);
            string rejectReasonCode = getSeg(currentLine, 3);
            string followUpActionCode = getSeg(currentLine, 4);

            Console.WriteLine("");  //////////////////// writeln

            if (validRequestIndicator == "Y")
            {
                Console.WriteLine("This request was valid, however the transaction was rejected due to the following reason:"); //////////////////// writeln
            }
            else
            {
                Console.WriteLine("This request or an element in this request was NOT valid. This transaction was rejected due to the following reason:"); //////////////////// writeln
            }

            Console.WriteLine(rejectReasonCode + ": " + getIniValue("AAA03", rejectReasonCode)); //////////////////// writeln
            Console.WriteLine(getIniValue("AAA04", followUpActionCode)); //////////////////// writeln
            Console.WriteLine(""); //////////////////// writeln

            // addToBothCSVs(header,'Error',rejectReasonCode+': '+getIniValue('AAA03',rejectReasonCode)); //////////////////// addToBothCSVs
        }

        public static void iii()
        {
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
            Console.WriteLine("     " + codeListQualifierCode + ": " + industryCode); //////////////////// writeln

            // addToBothCSVs(header,'QualCode',codeListQualifierCode); //////////////////// addToBothCSVs
            // addToBothCSVs(header,'Code', industryCode); //////////////////// addToBothCSVs
        }
       
        public static void eb()
        {
            ebCount++;
            List<string> serviceTypeCodes = new List<string>();
            string eligibilityInfo;
            string coverageLevelCode;
            string serviceTypeCode;
            string insuranceTypeCode;
            string planCoverageDesc;
            string timePeriodQual;
            string benefitAmount;
            string benefitPercent;
            string quantityQualifier;
            string benefitQuantity;
            string authOrCertIndicator;
            string inPlanNetworkIndicator;
            string productServiceIDQual;
            string procedureCode;
            string procedureModifier;
            string diagPointer = "";

            eligibilityInfo = getIniValue("EB01", getSeg(currentLine, 1));
            coverageLevelCode = getIniValue("EB02", getSeg(currentLine, 2));
            int i = 1;
            while (getsub(currentLine, repetitionDelim, 3, i) != "")
            {
               serviceTypeCodes.Add(getIniValue("EB03", getsub(currentLine, repetitionDelim, 3, i)));
               i++;
            }
            serviceTypeCode = getIniValue("EB03", getSeg(currentLine, 3));
            insuranceTypeCode = getIniValue("EB04", getSeg(currentLine, 4));
            planCoverageDesc = getSeg(currentLine, 5);
            timePeriodQual = getIniValue("EB06", getSeg(currentLine, 6));
            //this is for Medicare - they are returning 29 - Remaining for AD, AE, and AF, so I have to chnage it to Used
            if ((InsName == "CMS") && (timePeriodQual == "Remaining") && ((getSeg(currentLine, 3) == "AD") || (getSeg(currentLine, 3) == "AE") || (getSeg(currentLine, 3) == "AF")))
            {
                timePeriodQual = "Used";
            }
               
            benefitAmount = getSeg(currentLine, 7);
            benefitPercent = getSeg(currentLine, 8);
            quantityQualifier = getIniValue("EB09", getSeg(currentLine, 9));
            benefitQuantity = getSeg(currentLine, 10);
            authOrCertIndicator = getSeg(currentLine, 11);
            inPlanNetworkIndicator = getSeg(currentLine, 12);
            productServiceIDQual = getIniValue("EB13-1", getsub(currentLine, subElementDelim, 13, 1));
            procedureCode = getsub(currentLine, subElementDelim, 13, 2);
            procedureModifier = getsub(currentLine, subElementDelim, 13, 3);

            for (i = 4; i <= 6; i++)
            {
                if (getsub(currentLine, subElementDelim, 13, i) != "")
                    procedureModifier = procedureModifier + "," + getsub(currentLine, subElementDelim, 13, i);
            }
            for (i = 1; i <= 4; i++)
            {
                if (getsub(currentLine, subElementDelim, 14, i) != "")
                {
                    diagPointer += "," + getsub(currentLine, subElementDelim, 14, i);
                }
            }
            
            if (getIniValue("EB03", getSeg(lastEB, 3)) != serviceTypeCode)
            {
                Console.WriteLine(""); //////////////////// writeln
                for (i = 0; i < serviceTypeCodes.Count - 1; i++) 
                {
                    Console.WriteLine(serviceTypeCodes[i] + " - " + getsub(currentLine, repetitionDelim, 3, i + 1)); //////////////////// writeln
                }
                lastEB = "";
            }

            if (getIniValue("EB01", getSeg(lastEB, 1)) != eligibilityInfo)
            {
                if (eligibilityInfo.ToUpper().IndexOf("ACTIVE") != 0)
                {
                    Console.WriteLine(" Status: " + eligibilityInfo); //////////////////// writeln
                }
                else
                {
                    Console.WriteLine(" " + eligibilityInfo); //////////////////// writeln
                }
                lastEB = "EB*" + eligibilityInfo;
            }

            if ((getIniValue("EB02", getSeg(lastEB, 2)) != coverageLevelCode) && (coverageLevelCode != ""))
            {
                Console.WriteLine(" Coverage Level: " + coverageLevelCode); //////////////////// writeln
                lastEB = "EB*" + eligibilityInfo;
            }

            if (((getIniValue("EB05", getSeg(lastEB, 5)) != planCoverageDesc) && (planCoverageDesc != "")) || ((getIniValue("EB04", getSeg(lastEB, 4)) != insuranceTypeCode) && (insuranceTypeCode != "")))
            {
                if (planCoverageDesc != "")
                {
                    insuranceTypeCode = insuranceTypeCode + " (" + planCoverageDesc + ")";
                }
                Console.WriteLine("  " + insuranceTypeCode); //////////////////// writeln
                lastEB = "EB*" + eligibilityInfo + "*" + coverageLevelCode;
            }

            if (benefitAmount != "")
            {
                Console.Write("    Amount: $" + benefitAmount); //////////////////// write
            }
            if (benefitPercent != "") 
            {
                // Console.Write("    Percent: " + benefitPercent); //////////////////// write
                Console.Write("    Percent: " + float.Parse(benefitPercent) * 100 + "%"); //////////////////// write
            }
            if ((benefitAmount != "") || (benefitPercent != ""))
            {
                if (timePeriodQual == "Remaining")
                {
                    Console.Write(" " + timePeriodQual); //////////////////// write
                }
                else if (timePeriodQual != "")
                {
                    Console.Write("  per " + timePeriodQual); //////////////////// write
                }
                Console.WriteLine(""); //////////////////// writeln
            }
            else
            {
                if (timePeriodQual != "")
                {
                    Console.WriteLine("    " + timePeriodQual); //////////////////// writeln
                }
            }

            if ((benefitQuantity != "") && (quantityQualifier != ""))
            {
                Console.WriteLine("      " + benefitQuantity + " " + quantityQualifier); //////////////////// writeln
            }
            else if  (quantityQualifier != "") 
            {
                Console.WriteLine("      " + quantityQualifier); //////////////////// writeln
            }
            if (authOrCertIndicator != "")
            {
                Console.WriteLine("       Authorization or Certificate Required: " + authOrCertIndicator); //////////////////// writeln
            }
            if (inPlanNetworkIndicator == "Y")
            {
                Console.WriteLine("       Benefits are In-Plan-Network"); //////////////////// writeln
            }
            else if (inPlanNetworkIndicator == "N")
            {
                Console.WriteLine("       Benefits are Out-Of-Plan-Network"); //////////////////// writeln
            }
            if (productServiceIDQual != "")
            {
                if (procedureCode == "G0180")
                {
                    Console.WriteLine("   Home Health Certification (G0180)"); //////////////////// writeln
                }
                else if (procedureCode == "G0179")
                {
                    Console.WriteLine("   Home Health Recertification (G0179)"); //////////////////// writeln
                }
                else
                {
                    Console.WriteLine("       " + productServiceIDQual + ": " + procedureCode); //////////////////// writeln
                }

            }

            if (procedureModifier != "")
            {
                Console.WriteLine("        Modifiers: " + procedureModifier); //////////////////// writeln
            }
            if (diagPointer != "")
            {
                Console.WriteLine("        Diag Pointers: " + diagPointer); //////////////////// writeln
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

                if (benefitPercent != "")
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
            Console.WriteLine("     " + quantity + quantity2 + quantity3 + quantity4); //////////////////// writeln

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
            string freeFormMessageText = getSeg(currentLine, 1);
            Console.WriteLine("     " + freeFormMessageText); //////////////////// writeln

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
            {currentLine = getNextLine();
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
                    /*                    j = s2.IndexOf("\"\"");
                                        while (j != -1)
                                        {
                                            s2[j] = "¾";
                                            s2[j+1] = "¾";
                                            j = s2.IndexOf("\"\"");
                                        }*/
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
            /*            j = result.IndexOf("¾¾");
                        while (j != -1)
                        {
                            result[j] = '"';
                            result.Remove(j+1,1);
                            j = result.IndexOf("¾¾");
                        }*/
            return result.Trim();
        }
    }
}
