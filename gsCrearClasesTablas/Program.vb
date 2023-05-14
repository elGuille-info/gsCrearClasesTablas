' Program.vb como punto de entrada del programa.            (14/may/23 19.04)
Friend Module Program

    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Try
            Application.Run(New Form1())
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error no controlado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

End Module
