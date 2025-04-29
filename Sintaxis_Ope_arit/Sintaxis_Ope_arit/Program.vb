Imports System

' ParserConsola.vb - Análisis sintáctico por consola en VB.NET

Module ParserConsola
    ' Nodo del Árbol Sintáctico Abstracto (AST)
    Public Class ASTNode
        Public Property Value As String
        Public Property Left As ASTNode
        Public Property Right As ASTNode

        Public Sub Print(Optional ByVal indent As String = "")
            Console.WriteLine(indent & Value)
            If Left IsNot Nothing Then Left.Print(indent & "   ")
            If Right IsNot Nothing Then Right.Print(indent & "   ")
        End Sub
    End Class

    ' Analizador Sintáctico Recursivo Descendente
    Public Class Parser
        Private tokens As List(Of String)
        Private pos As Integer = 0

        Public Function Parse(expression As String) As ASTNode
            tokens = Tokenize(expression)
            pos = 0
            Return ParseExpression()
        End Function

        Private Function Tokenize(expr As String) As List(Of String)
            Dim list As New List(Of String)
            Dim sb As New Text.StringBuilder
            For Each c As Char In expr
                If Char.IsWhiteSpace(c) Then
                    If sb.Length > 0 Then
                        list.Add(sb.ToString())
                        sb.Clear()
                    End If
                ElseIf "()+-*/".Contains(c) Then
                    If sb.Length > 0 Then
                        list.Add(sb.ToString())
                        sb.Clear()
                    End If
                    list.Add(c.ToString())
                ElseIf Char.IsDigit(c) Then
                    sb.Append(c)
                Else
                    Throw New Exception("Caracter no válido: " & c)
                End If
            Next
            If sb.Length > 0 Then list.Add(sb.ToString())
            Return list
        End Function

        Private Function ParseExpression() As ASTNode
            Return ParseTerm()
        End Function

        Private Function ParseTerm() As ASTNode
            Dim left = ParseFactor()
            While pos < tokens.Count AndAlso (tokens(pos) = "+" OrElse tokens(pos) = "-")
                Dim op = tokens(pos)
                pos += 1
                Dim right = ParseFactor()
                left = New ASTNode With {.Value = op, .Left = left, .Right = right}
            End While
            Return left
        End Function

        Private Function ParseFactor() As ASTNode
            Dim left = ParsePrimary()
            While pos < tokens.Count AndAlso (tokens(pos) = "*" OrElse tokens(pos) = "/")
                Dim op = tokens(pos)
                pos += 1
                Dim right = ParsePrimary()
                left = New ASTNode With {.Value = op, .Left = left, .Right = right}
            End While
            Return left
        End Function

        Private Function ParsePrimary() As ASTNode
            If pos >= tokens.Count Then Throw New Exception("Expresión incompleta")

            Dim token = tokens(pos)

            If token = "(" Then
                pos += 1
                Dim expr = ParseExpression()
                If pos >= tokens.Count OrElse tokens(pos) <> ")" Then
                    Throw New Exception("Falta paréntesis de cierre")
                End If
                pos += 1
                Return expr
            ElseIf IsNumeric(token) Then
                pos += 1
                Return New ASTNode With {.Value = token}
            Else
                Throw New Exception("Token inesperado: " & token)
            End If
        End Function
    End Class

    ' Punto de entrada del programa
    Sub Main()
        Console.WriteLine("Ingrese una expresión aritmética:")
        Dim input As String = Console.ReadLine()

        Try
            Dim parser As New Parser()
            Dim ast As ASTNode = parser.Parse(input)
            Console.WriteLine(vbCrLf & "Árbol Sintáctico:" & vbCrLf)
            ast.Print()
        Catch ex As Exception
            Console.WriteLine("Error de análisis: " & ex.Message)
        End Try

        Console.WriteLine(vbCrLf & "Presione cualquier tecla para salir...")
        Console.ReadKey()
    End Sub
End Module