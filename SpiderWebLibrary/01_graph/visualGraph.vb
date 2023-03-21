Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare

Namespace graphRepresentaions
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : visualGraph
    ''' 
    ''' <summary>
    ''' Preforms recursiveShadowCasting to generate a visualGraph
    ''' </summary>
    ''' <remarks>
    ''' This implementation is based on a Java implementation by Eben Howard:
    ''' http://www.roguebasin.com/index.php?title=FOV_using_recursive_shadowcasting_-_improved
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   06/03/2014 created
    ''' </history>
    ''' 
    Public Class visualGraph
        Inherits graphVertexList

        ''' <summary>
        ''' State (invalid, solid, void) of gridCell as Integer(x)(y)
        ''' </summary>
        ''' <remarks></remarks>
        Protected grid()() As State

        ''' <summary>
        ''' All possible viewing directions.
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly direction(,) As Integer = New Integer(3, 1) {{1, 1}, {-1, 1}, {1, -1}, {-1, -1}}

        ''' <summary>
        ''' representation of the possible states of each graphVertex / gridCell
        ''' </summary>
        ''' <remarks></remarks>
        Enum State As Integer
            invalid = -1
            void = 0
            solid = 1
        End Enum

        Public ReadOnly NB4(,) As Integer = New Integer(3, 1) {{1, 0}, {0, -1}, {0, 1}, {-1, 0}}
        Public ReadOnly NB8(,) As Integer = New Integer(7, 1) {{1, -1}, {1, 0}, {1, 1}, {0, -1}, {0, 1}, {-1, -1}, {-1, 0}, {-1, 1}}
        ' -------------------- Constructor --------------------------

        ''' <summary>
        ''' Construct an invalid visualGraph
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
            Me.setup(0, 0)
        End Sub

        ''' <summary>
        ''' Construct a new visualGraph
        ''' </summary>
        ''' <param name="w">Width of the grid (x)</param>
        ''' <param name="h">Height of the grid (y)</param>
        ''' <remarks>Will set all gridCells to State.void</remarks>
        Public Sub New(ByVal w As Integer, ByVal h As Integer)
            MyBase.New()
            Me.setup(w, h)
        End Sub

        ''' <summary>
        ''' Construct a new visualGraph.
        ''' </summary>
        ''' <param name="gV">Existing visualGraph</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal gV As visualGraph)
            MyBase.New(gV.getVertexList)
            Me.grid = gV.gridCell
        End Sub

        ' -------------------- Public --------------------------
        ''' <summary>
        ''' Change the State of a graphVertex / gridCell
        ''' </summary>
        ''' <param name="x">x coordinate in the grid</param>
        ''' <param name="y">y coordinate in the grid</param>
        ''' <value>If x, y are larger than the grid, nothing happens.</value>
        ''' <returns>Returns the state of the specified gridCell if x or yare larger than the grid, State.invalid will be returned</returns>
        ''' <remarks></remarks>
        Property gridCell(ByVal x As Integer, ByVal y As Integer) As State
            Get
                If (x < Me.grid.Count And y < Me.grid(x).Count) Then
                    Return Me.grid(x)(y)
                Else
                    Return State.invalid
                End If
            End Get
            Set(value As State)
                If (x < Me.grid.Count And y < Me.grid(x).Count) Then
                    Me.grid(x)(y) = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Change the State of a graphVertex / gridCell
        ''' </summary>
        ''' <param name="p">position in the grid</param>
        ''' <value>If p is larger than the width*height or smaller than 0, nothing happens.</value>
        ''' <returns>Returns the state of the specified gridCell if p is not within the grid, State.invalid will be returned</returns>
        ''' <remarks></remarks>
        Property gridCell(ByVal p As Integer) As State
            Get
                Return Me.gridCell(Me.pX(p), Me.pY(p))
            End Get
            Set(value As State)
                Me.gridCell(Me.pX(p), Me.pY(p)) = value
            End Set
        End Property

        ''' <summary>
        ''' Get the States of the visualGraphGrid
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns a (x)(y) Integer Array</returns>
        ''' <remarks></remarks>
        Property gridCell() As State()()
            Get
                Return Me.grid
            End Get
            Set(value As State()())
                Me.grid = value
            End Set
        End Property

        ''' <summary>
        ''' Get the States of the visualGraphGrid
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the States as List</returns>
        ''' <remarks></remarks>
        ReadOnly Property gridCellList() As List(Of Integer)
            Get
                Dim retList As New List(Of Integer)
                For iY As Integer = 0 To Me.height - 1
                    For iX As Integer = 0 To Me.width - 1
                        retList.Add(Me.grid(iX)(iY))
                    Next
                Next
                Return retList
            End Get
        End Property

        ReadOnly Property gridCellNB(ByVal p As Integer, Optional ByVal nb As Integer = 4) As Integer() 'achtung hier ist ein fehler...
            Get
                Dim posNB() As Integer
                If nb = 8 Then
                    ReDim posNB(7)
                    For i As Integer = 0 To 7
                        posNB(i) = (p + Me.NB8(i, 0) * width + Me.NB8(i, 1))
                        If Math.Floor(posNB(i) / Me.width) <> Math.Floor(p / Me.width) + Me.NB8(i, 0) Then
                            posNB(i) = -posNB(i)
                        End If
                    Next
                Else
                    ReDim posNB(3)
                    For i As Integer = 0 To 3
                        posNB(i) = (p + Me.NB4(i, 0) * width + Me.NB4(i, 1))
                        If Math.Floor(posNB(i) / Me.width) <> Math.Floor(p / Me.width) + Me.NB4(i, 0) Then
                            posNB(i) = -posNB(i)
                        End If
                    Next
                End If

                Return posNB
            End Get
        End Property

        ReadOnly Property pX(ByVal p As Integer) As Integer
            Get
                Return p Mod Me.width
            End Get
        End Property

        ReadOnly Property pY(ByVal p As Integer) As Integer
            Get
                Return Math.Floor(p / Me.width)
            End Get
        End Property

        ''' <summary>
        ''' Get the width of the visualGraphGrid
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property width() As Integer
            Get
                Return Me.grid.Count
            End Get
        End Property

        ''' <summary>
        ''' Get the height of the visualGraphGrid
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property height() As Integer
            Get
                If Me.grid.Count > 0 Then
                    Return Me.grid(0).Count
                End If
                Return 0
            End Get
        End Property

        ReadOnly Property circumference(ByVal p As Integer, Optional ByVal nb As Integer = 4) As Integer
            Get
                Dim circ As Integer = 0
                If Me.gridCell(p) = State.void Then
                    For Each e_NB As Integer In Me.gridCellNB(p, nb)
                        If Me.Item(p).findNB(e_NB) < 0 And e_NB <> p Then
                            circ += 1
                            Exit For
                        End If
                    Next
                End If
                For Each e As Integer In Me.Item(p).neighbours
                    For Each e_NB As Integer In Me.gridCellNB(e, nb)
                        If Me.Item(p).findNB(e_NB) < 0 And e_NB <> p Then
                            circ += 1
                            Exit For
                        End If
                    Next
                Next
                Return circ
            End Get
        End Property

        ''' <summary>
        ''' Calculate the visible area from a point
        ''' </summary>
        ''' <param name="p">graphVertex of the visualGraph</param>
        ''' <value></value>
        ''' <returns>Returns the number of gridCells (= the outDegree of the graphVertex) visible from the sepcified position.</returns>
        ''' <remarks></remarks>
        ReadOnly Property area(ByVal p As Integer) As Integer
            Get
                Return Me.Item(p).outDegree
            End Get
        End Property

        ' -------------------- Public --------------------------


        ''' <summary>
        ''' Construct a visualGraph from the specified x,y coordinates.
        ''' </summary>
        ''' <param name="sX">x coordinate in the grid</param>
        ''' <param name="sY">y coordinate in the grid</param>
        ''' <remarks></remarks>
        Public Sub recursive(ByVal sX As Integer, ByVal sY As Integer, Optional ByVal edgeWeight As Integer = 0)
            If Me.gridCell(sX, sY) = State.void Then
                For i As Integer = 0 To (Me.direction.Length / 2) - 1
                    castRecursive(sX, sY, 1, 1.0, 0.0, 0, Me.direction(i, 0), Me.direction(i, 1), 0, edgeWeight)
                    castRecursive(sX, sY, 1, 1.0, 0.0, Me.direction(i, 0), 0, 0, Me.direction(i, 1), edgeWeight)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Constructs the complete visualGraph.
        ''' </summary>
        ''' <remarks>Will call recursive(x,y) form all possible coordinates of the grid</remarks>
        Public Sub recursiveAll(Optional ByVal edgeWeight As Integer = 0)
            For iX As Integer = 0 To Me.grid.Count - 1
                For iY As Integer = 0 To Me.grid(iX).Count - 1
                    If Me.grid(iX)(iY) = State.void Then
                        Me.recursive(iX, iY, edgeWeight)
                    End If
                Next
            Next
        End Sub

        Public Overloads Function toString()
            Return "visualGraph: " & Me.width & "*" & Me.height & ", " & Me.vertexCount & " vertices, " & Me.edgeCount & " edges"
        End Function

        ''' <summary>
        ''' Setup visualGraphGrid
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub setup(ByVal w As Integer, ByVal h As Integer)
            Me.vertexList = New List(Of graphVertex)
            For i As Integer = 0 To (w * h) - 1
                Me.add(New graphVertex(i))
            Next
            Me.grid = New State(w - 1)() {}
            For i As Integer = 0 To Me.grid.Count - 1
                Me.grid(i) = New State(h - 1) {}
            Next
            For iX As Integer = 0 To Me.grid.Count - 1
                For iY As Integer = 0 To Me.grid(iX).Count - 1
                    Me.grid(iX)(iY) = State.void
                Next
            Next
        End Sub

        ' -------------------- Private --------------------------

        ''' <summary>
        ''' Construct a visualGraph in one of eight possible directions
        ''' </summary>
        ''' <param name="sX">x coordinate in the grid</param>
        ''' <param name="sY">y coordinate in the grid</param>
        ''' <param name="row">current row in the shadow casting</param>
        ''' <param name="s">starting angle</param>
        ''' <param name="e">end angle</param>
        ''' <param name="xx">xx direction</param>
        ''' <param name="xy">yy direction</param>
        ''' <param name="yx">yx direction</param>
        ''' <param name="yy">yy direction</param>
        ''' <remarks></remarks>
        Private Sub castRecursive(ByVal sX As Integer, ByVal sY As Integer, ByVal row As Integer, _
                         ByVal s As Double, ByVal e As Double, _
                         ByVal xx As Integer, ByVal xy As Integer, _
                         ByVal yx As Integer, ByVal yy As Integer, Optional ByVal edgeWeight As Integer = 0)

            Dim newStart As Double = 0D
            If (s < e) Then Return

            Dim blocked As Boolean = False
            Dim dist As Integer = row
            Dim sel As Integer
            Dim deltaY, cX, cY As Integer
            Dim leftslope, rightSlope As Double
            Dim vis, visLeft, visRight As Double
            Dim gE As graphEdge

            While (Not blocked)
                deltaY = -dist
                For deltaX As Integer = -dist To 0
                    cX = sX + deltaX * xx + deltaY * xy
                    cY = sY + deltaX * yx + deltaY * yy
                    leftslope = (deltaX - 0.5D) / (deltaY + 0.5D)
                    rightSlope = (deltaX + 0.5D) / (deltaY - 0.5D)

                    If (Not (cX >= 0 And cY >= 0 And cX < Me.width And cY < Me.height) Or s < rightSlope) Then
                        Continue For
                    ElseIf (s < e) Then
                        Return
                    ElseIf (e >= leftslope) Then
                        Exit For
                    End If

                    If Me.grid(cX)(cY) = State.void Then
                        gE = New graphEdge(sY * Me.width + sX, cY * Me.width + cX)
                        Select Case edgeWeight
                            Case 1 ' visiblity % 
                                vis = Math.Abs(leftslope - rightSlope)
                                visLeft = Math.Abs(s - rightSlope)
                                visRight = Math.Abs(e - leftslope)
                                If visLeft < vis And visRight < vis Then
                                    gE.Cost = Math.Abs(e - s) / vis
                                ElseIf visLeft < vis Then
                                    If deltaX <> -dist Then
                                        gE.Cost = visLeft / vis
                                    Else
                                        'noch nicht ganz richtig
                                        sel = Me.Item(gE.A).findNB(gE.B)
                                        If sel < 0 Then
                                            gE.Cost = 0.5
                                        Else
                                            Me.Item(gE.A).cost(sel) += 0.5
                                        End If

                                    End If
                                ElseIf visRight < vis And deltaX <> 0 Then
                                        gE.Cost = visRight / vis
                                Else
                                        gE.Cost = 1.0
                                End If
                            Case Else ' euclidian Distance
                                gE.Cost = Math.Pow(Math.Pow(dist, 2) + Math.Pow(deltaX, 2), 0.5)
                        End Select
                        Me.insert(gE)
                    End If

                    If blocked Then
                        If Me.grid(cX)(cY) = State.solid Then
                            newStart = rightSlope
                            Continue For
                        Else
                            blocked = False
                            s = newStart
                        End If
                    Else
                        If Me.grid(cX)(cY) = State.solid Then
                            blocked = True
                            castRecursive(sX, sY, dist + 1, s, leftslope, xx, xy, yx, yy, edgeWeight)
                            newStart = rightSlope
                        End If
                    End If
                Next
                If (Not (cX >= 0 And cY >= 0 And cX < Me.width And cY < Me.height) Or s < rightSlope) Then
                    Return
                End If
                dist += 1
            End While


        End Sub


    End Class
End Namespace
