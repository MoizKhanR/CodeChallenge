namespace Report_Safety_Analyzer
{
    public partial class Form1 : Form
    {
        string Source_Path = AppDomain.CurrentDomain.BaseDirectory + "\\Reports.txt";
        string[]? Reports;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Reports != null)
                {
                    int Safe_Reports = 0;

                    //Calculate how many are safe
                    for (int Index = 0; Index < Reports.Length; Index++)
                    {
                        bool Is_Safe = Report_Status(Reports, Index);

                        if (Is_Safe)
                        {
                            Safe_Reports++;
                        }
                        if(!Is_Safe)
                        {
                            bool Is_Safe_With_Dampener = Dampener(Reports, Index);

                            if(Is_Safe_With_Dampener)
                            {     Safe_Reports++; }
                        }
                    }

                    MessageBox.Show($"Total Safe Reports (with Dampener): {Safe_Reports}");
                }

                //bool Is_Safe = Report_Status(Reports, Reports.Length-1);

                //MessageBox.Show($"Report {Reports.Length} is {(Is_Safe ? "Safe" : "Not Safe")}");   

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public bool Report_Status(string[] Report, int Index)
        {
            bool Is_Safe = false;

            try
            {
                
                //Acquire each number as a token per report

                string[] Tokens = Report[Index].Split(' ');
                int Node_A;
                int Node_B;
                int Difference = 0;
                int Expected_Trend = -1; //0 means Decreasing 1 means Increasing

                //Traverse the report and check if it is safe

                for (int i = 0; i < Tokens.Length; i++)
                {
                    //Tokens in question
                    if ((i + 1) < Tokens.Length)
                    {
                        Node_A = Convert.ToInt32(Tokens[i]);
                        Node_B = Convert.ToInt32(Tokens[i + 1]);
                        Difference = Node_A - Node_B;
                        
                        //Right at the first iteration we determine the expected trend
                        if (i == 0)
                        {
                            if (Difference < 0)
                            {
                                Expected_Trend = 0; //Increasing
                            }
                            if (Difference > 0)
                            {
                                Expected_Trend = 1; //Decreasing 
                            }
                            if (Difference == 0)
                            {
                                return false; //immediately return false because the report is not safe
                            }
                        }
                    }

                    //Depending upon the Difference, we can see if it IS and SHOULD Decrease or Increase

                    switch (Expected_Trend)
                    {
                        //Depending upon the trend, perform the appropriate checks
                        case 0:
                            {                            
                                //If the trend shifts at all, the report is not safe
                                if (Difference > 0)
                                {
                                    return false;
                                }
                                //if there is no difference, that instantly means the report is not safe
                                if (Difference == 0)
                                {
                                    return false;
                                }
                                if (Math.Abs(Difference) >= 1 && Math.Abs(Difference) <= 3)
                                {
                                    Is_Safe = true;
                                }
                                else
                                {
                                    return false;
                                }

                                break;
                            }
                        case 1:
                            {                         
                                //If the trend shifts at all, the report is not safe
                                if (Difference < 0)
                                {
                                    return false; 
                                }

                                //if there is no difference, that instantly means the report is not safe
                                if (Difference == 0)
                                {
                                    return false;
                                }
                                if (Difference >= 1 && Difference <= 3)
                                {
                                    
                                    Is_Safe = true;
                                }
                                else
                                {
                                    return false;
                                }

                                break;
                            }

                    }
                }

                return Is_Safe;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return Is_Safe;
        }

        public bool Dampener(string[] Report, int Index)
        {
            if (Report is not null)
            {
                string[] Tokens = Report[Index].Split(' ');

                for (int Skip = 0; Skip < Tokens.Length; Skip++)
                {
                    //Damepener applied
                    string[] Dampened = Tokens.Where((_, Position) => Position != Skip).ToArray();

                    //Insert it back into a shallow copy of the report array (original stays untouched)
                    string[] ReportsCopy = (string[])Report.Clone();
                    ReportsCopy[Index] = string.Join(" ", Dampened);

                    //Check if the report is safe with the dampener applied
                    bool Is_Safe = Report_Status(ReportsCopy, Index);

                    if (Is_Safe)
                    {
                        return true;
                    }

                }
            }            

            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Reports = File.ReadAllLines(Source_Path);
            Reports_Text_Box.Lines = Reports;
        }
    }
}
