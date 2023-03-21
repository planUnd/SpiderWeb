Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphRepresentaions
Imports SpiderWebLibrary.clustering
Imports MathNet.Numerics.LinearAlgebra
Imports MathNet.Numerics.LinearAlgebra.Factorization

Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : spectralGraphMatrix
    ''' 
    ''' <summary>
    ''' Spectral Graph MAtrix 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   28/07/2014 created
    ''' [Richard Schaffranek]   09/11/2014 added: removeDegenerated, removeZero
    ''' [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' [Richard Schaffranek]   14/01/2015 added: truncate; renamed: eigenVectorRow,eigsenVectorRows -> featureVector, featureVectors
    ''' </history>
    ''' 
    Public Class spectralGraphMatrix

        Private EV() As Double
        Private EVec() As Vector(Of Double)
        Private s() As signCorrection
        Private MT As graphMatrix.matrixType

        Enum signCorrection
            pos = 1
            negativ = -1
        End Enum

        Public Sub New(ByVal gM As graphMatrix)
            Dim ES As Evd(Of Double)
            ES = gM.getMatrix.Evd(Symmetricity.Symmetric)
            Me.MT = gM.type
            Me.EV = ES.D.Diagonal.ToArray()
            ReDim Me.s(gM.dimension - 1)
            ReDim Me.EVec(gM.dimension - 1)
            For i As Integer = 0 To gM.dimension - 1
                Me.EVec(i) = ES.EigenVectors.Column(i)
                Me.s(i) = signCorrection.pos
            Next
            If Me.MT >= graphMatrix.matrixType.gausianWeightedMatrix Then
                Array.Reverse(Me.EV)
                Array.Reverse(Me.EVec)
                Array.Reverse(Me.s)
            End If
        End Sub

        ' ----------------------- Properties ------------------------
        ''' <summary>
        ''' Column Count
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the number of Columns</returns>
        ''' <remarks></remarks>
        ReadOnly Property columnCount() As Integer
            Get
                Return Me.EVec.Length
            End Get
        End Property

        ''' <summary>
        ''' Row Count
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the number of Rows</returns>
        ''' <remarks></remarks>
        ReadOnly Property rowCount() As Integer
            Get
                If Me.columnCount > 0 Then
                    Return Me.EVec(0).Count
                End If
                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Count the number of zero Eigenvalues
        ''' </summary>
        ''' <param name="tol">Tolerance parameter</param>
        ''' <value></value>
        ''' <returns>Returns the number of Eigenvalues with the number of 0.</returns>
        ''' <remarks>If the graphMatrix, the spectralGrpah class was constructed with, was a (*) Laplacian Matrix 
        ''' this is the number of connected components of an undirected Graph.
        ''' </remarks>
        ReadOnly Property countZero(Optional tol As Double = 0.0000000001) As Integer
            Get
                Dim count As Integer = 0
                For i As Integer = 0 To Me.columnCount
                    If Math.Round(Me.eigenValue(i) / tol) = 0 Then
                        count += 1
                    End If
                Next
                Return count
            End Get
        End Property

        ''' <summary>
        ''' Get the i'th eigenValue
        ''' </summary>
        ''' <param name="i">index of the eigenValue</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property eigenValue(ByVal i As Integer) As Double
            Get
                Return Me.EV(i)
            End Get
        End Property

        ''' <summary>
        ''' Returns the eigenValue as Double Array
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property eigenValue() As Double()
            Get
                Return Me.EV.ToArray()
            End Get
        End Property

        ''' <summary>
        ''' Get the i'th eigenVector
        ''' </summary>
        ''' <param name="i">index of the eigenVector</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property eigenVector(ByVal i As Integer) As Vector(Of Double)
            Get
                Return Me.EVec(i) * Me.s(i)
            End Get
        End Property

        ''' <summary>
        ''' Get all eigenVectors of the graphMatrix
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property eigenVector() As Vector(Of Double)()
            Get
                Dim tmpEVec(Me.columnCount) As Vector(Of Double)
                For i As Integer = 0 To Me.columnCount - 1
                    tmpEVec(i) = Me.eigenVector(i)
                Next
                Return tmpEVec
            End Get
        End Property

        ReadOnly Property EVhistograms(Optional ByVal s As Integer = 1, Optional ByVal b As Integer = -1) As Vector(Of Double)()
            Get
                Dim tmpH(Me.columnCount) As Vector(Of Double)
                For i As Integer = 0 To Me.columnCount - 1
                    tmpH(i) = Me.EVhistogram(i, b, s)
                Next
                Return tmpH
            End Get
        End Property

        ReadOnly Property EVhistogram(ByVal i As Integer, Optional ByVal b As Integer = -1, Optional ByVal s As Integer = 1) As Vector(Of Double)
            Get
                If b = -1 Then
                    b = Math.Floor(((Me.rowCount) ^ (4 / 3)) / 2)
                End If
                b = Math.Min(b, Me.rowCount)

                Dim tmpHist As Vector(Of Double)
                Dim tmpEV As Vector(Of Double) = Me.eigenVector(i)
                Dim selB As Integer

                tmpHist = Vector(Of Double).Build.Dense(b)

                For j As Integer = 0 To Me.rowCount - 1
                    selB = Math.Floor((tmpEV(j) * s - 1.0) / 2.0 * b)
                    tmpHist(selB) += 1
                Next

                tmpHist = tmpHist / tmpHist.Sum()

                Return tmpHist
            End Get
        End Property

        ''' <summary>
        ''' Get the i'th row of the eigenvectors
        ''' </summary>
        ''' <param name="i">i'th row of the eigenvectors</param>
        ''' <param name="d">dimension of the rowVector (number of eigenvectors taken into account)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property featureVector(ByVal i As Integer, Optional ByVal d As Integer = Integer.MaxValue) As Vector(Of Double)
            Get
                d = Math.Min(Me.columnCount, d)
                Dim tmpVec As Vector(Of Double) = Vector(Of Double).Build.Dense(d)
                For j As Integer = 0 To d - 1
                    tmpVec(j) = Me.EVec(j)(i) * Me.s(j)
                Next
                Return tmpVec
            End Get
        End Property

        ''' <summary>
        ''' Get all rows of the eigenvectors
        ''' </summary>
        ''' <param name="d">dimension of the rowVector (number of eigenvectors taken into account)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property featureVectors(Optional ByVal d As Integer = Integer.MaxValue) As Vector(Of Double)()
            Get
                Dim sP(Me.rowCount - 1) As Vector(Of Double)
                For i As Integer = 0 To Me.rowCount - 1
                    sP(i) = Me.featureVector(i, d)
                Next
                Return sP
            End Get
        End Property

        ''' <summary>
        ''' Dominant sign of an eigenvector
        ''' </summary>
        ''' <param name="i">i'th eigenvector</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>dominant Sign takes the currently computed sign correction into account</remarks>
        ReadOnly Property dominantSign(ByVal i As Integer) As Integer
            Get
                Dim n As Integer

                For Each v As Double In Me.eigenVector(i)
                    If v < 0 Then
                        n += 1
                    Else
                        n -= 1
                    End If
                Next

                If n > 0 Then
                    Return -1
                End If
                Return 1
            End Get
        End Property

        ''' <summary>
        ''' Get or set the sign correction for a specific eigenvector
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property sign(i) As signCorrection
            Get
                Return Me.s(i)
            End Get
            Set(value As signCorrection)
                Me.s(i) = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set the sign correction for all eigenvectors
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property sign() As signCorrection()
            Get
                Return Me.s
            End Get
            Set(value As signCorrection())
                If value.Length = Me.s.Length Then
                    Me.s = value
                End If
            End Set
        End Property

        ' ---------------------- Sub ---------------------

        ''' <summary>
        ''' Reset all signs to signCorrection.pos
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub resetSigns()
            For i As Integer = 0 To Me.columnCount - 1
                Me.s(i) = signCorrection.pos
            Next
        End Sub

        ''' <summary>
        ''' Remove all Eigenvalues and Eigenvectors with an Eigenvalue of 0
        ''' </summary>
        ''' <param name="tol">numerical Tolerance</param>
        ''' <remarks></remarks>
        Public Sub removeZero(Optional tol As Double = 0.0000000001)
            Dim map As New List(Of Integer)(Me.columnCount)
            For i As Integer = 0 To Me.columnCount - 1
                If Math.Round(Me.eigenValue(i) / tol) <> 0 Then
                    map.Add(i)
                End If
            Next
            map.TrimExcess()
            Me.remap(map.ToArray())
        End Sub


        ''' <summary>
        ''' truncate eigenvectors
        ''' </summary>
        ''' <param name="k">number of eigenvector to keep</param>
        ''' <remarks></remarks>
        Public Sub truncate(ByVal k As Integer)
            Dim map As New List(Of Integer)(Me.columnCount)
            For i As Integer = 0 To Math.Min(k, Me.columnCount) - 1
                map.Add(i)
            Next
            map.TrimExcess()
            Me.remap(map.ToArray())
        End Sub

        ''' <summary>
        ''' Remove all Eigenvalues and Eigenvectors with a non unique Eigenvalue 
        ''' </summary>
        ''' <param name="tol">numerical Tolerance</param>
        ''' <remarks></remarks>
        Public Sub removeDegenerated(Optional tol As Double = 0.0000000001)
            Dim map As New List(Of Integer)(Me.columnCount)
            Dim deleted As Boolean = False
            For i As Integer = 0 To Me.columnCount - 2
                If Math.Round(Me.eigenValue(i) / tol) = Math.Round(Me.eigenValue(i + 1) / tol) Then
                    deleted = True
                ElseIf deleted Then
                    deleted = False
                Else
                    map.Add(i)
                End If
            Next
            If Not deleted Then
                map.Add(Me.columnCount - 1)
            End If
            map.TrimExcess()
            Me.remap(map.ToArray())
        End Sub

        ''' <summary>
        ''' </summary>
        ''' <param name="map"></param>
        ''' <remarks></remarks>
        Private Sub remap(ByVal map() As Integer)
            Dim tmpEV(map.Length - 1) As Double
            Dim tmpEVec(map.Length - 1) As Vector(Of Double)
            Dim tmpS(map.Length - 1) As signCorrection

            For i As Integer = 0 To map.Length - 1
                tmpEV(i) = Me.EV(map(i))
                tmpEVec(i) = Me.EVec(map(i))
                tmpS(i) = Me.s(map(i))
            Next

            Me.EV = tmpEV
            Me.EVec = tmpEVec
            Me.s = tmpS
        End Sub

        ' ---------------------- Function ---------------------
        ''' <summary>
        ''' Dominant Sign Correction
        ''' </summary>
        ''' <remarks> This is based on: 
        ''' Kosinov, Serhiy, and Terry Caelli. "Inexact multisubgraph matching using graph eigenspace and clustering models." Structural, Syntactic, and Statistical Pattern Recognition. Springer Berlin Heidelberg, 2002. 133-142.
        ''' http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.7.2143
        ''' </remarks>
        ''' <returns>
        ''' Returns False if the columnCount of the provided spectralGraphMatrix was smaller than the columnCount of this Graph.
        ''' </returns>
        Public Function dominantSignCorrection(ByVal sG As spectralGraphMatrix) As Boolean
            If sG.columnCount < Me.columnCount Then
                Return False
            End If

            Me.resetSigns()

            For i As Integer = 0 To Me.columnCount - 1
                If Me.dominantSign(i) <> sG.dominantSign(i) Then
                    Me.s(i) *= -1
                End If
            Next
            Return True
        End Function

        ''' <summary>
        ''' Minimizing Costs Sign Correction
        ''' </summary>
        ''' <remarks> This is based on: 
        ''' Shapiro, Larry S., and J. Michael Brady. "Feature-based correspondence: an eigenvector approach." Image and vision computing 10.5 (1992): 283-288.
        ''' http://image.ntua.gr/iva/files/ShapiroBrady_IVC1992%20-%20Feature-Based%20Correspondence-%20an%20Eigenvector%20Approach.pdf
        ''' </remarks>
        ''' <returns>
        ''' Returns False if the columnCount of the provided spectralGraphMatrix was smaller than the columnCount of this spectralGraphMatrix.
        ''' </returns>
        Public Function minimizeCostsSignCorrection(ByVal sG As spectralGraphMatrix) As Boolean
            If sG.columnCount < Me.columnCount Then
                Return False
            End If

            Me.resetSigns()

            Dim sP1(Me.rowCount) As Vector(Of Double)
            Dim sP2(sG.rowCount) As Vector(Of Double)

            Dim costsPos, costsNeg As Double

            For i As Integer = 1 To Me.columnCount
                sP1 = Me.featureVectors(i)
                sP2 = sG.featureVectors(i)
                costsPos = Me.signCosts(sP1, sP2)

                Me.sign(i - 1) = signCorrection.negativ
                sP1 = Me.featureVectors(i)
                sP2 = sG.featureVectors(i)

                costsNeg = Me.signCosts(sP1, sP2)
                If costsPos < costsNeg Then
                    Me.sign(i - 1) = signCorrection.pos
                End If
            Next
            Return True
        End Function

        Private Function signCosts(ByVal rV1() As Vector(Of Double), ByVal rV2() As Vector(Of Double)) As Double
            Dim costs As Double = 0
            For Each r1 As Vector(Of Double) In rV1
                For Each r2 As Vector(Of Double) In rV2
                    costs += (r1 - r2).L2Norm()
                Next
            Next
            Return costs
        End Function

        ''' <summary>
        ''' Histogram Sign Correction and Eigenvector Matching
        ''' </summary>
        ''' <param name="sG"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function histogramMatching(ByVal sG As spectralGraphMatrix, _
                                          ByVal dM As distanceClustering, _
                                          Optional ByVal tol As Integer = 10000) As Boolean
            If sG.columnCount < Me.columnCount Then
                Return False
            End If
            Me.resetSigns()

            Dim m(Me.columnCount) As Integer

            Dim histSG(sG.columnCount) As Vector(Of Double)
            Dim histMe(Me.columnCount) As Vector(Of Double)
            Dim histNMe(Me.columnCount) As Vector(Of Double)

            Dim dist As Matrix(Of Double)
            Dim sign As Matrix(Of Double)

            dist = MathNet.Numerics.LinearAlgebra.Matrix(Of Double).Build.Dense(sG.columnCount, Me.columnCount)
            sign = MathNet.Numerics.LinearAlgebra.Matrix(Of Double).Build.Dense(Me.columnCount, Me.columnCount)

            histSG = sG.EVhistograms()
            histMe = Me.EVhistograms()
            histNMe = Me.EVhistograms(-1)

            Dim tmpDist, tmpDistN As Double

            For i As Integer = 0 To histSG.Count - 1
                For ii As Integer = 0 To histMe.Count - 1
                    tmpDist = dM.dist(histSG(i), histMe(ii))
                    tmpDistN = dM.dist(histSG(i), histMe(ii))
                    If tmpDist <= tmpDistN Then
                        dist(i, ii) = Math.Round(tmpDist * tol)
                        sign(i, ii) = 1
                    Else
                        dist(i, ii) = Math.Round(tmpDistN * tol)
                        sign(i, ii) = -1
                    End If
                Next
            Next


            Return True
        End Function

        ''' <summary>
        ''' Select the dimension based on the profil likelihood of the eigenvalues.
        ''' </summary>
        ''' <returns>Dimension</returns>
        ''' <remarks>
        ''' Compare: Zuh, Mu and Ali, Ghodsi; "Automatic dimensionality selection from the scree plot via the use of profile likelihood." Computational statistics & Data Analysis 51.2 (2006): 918-930
        ''' http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.90.3768&rep=rep1&type=pdf
        ''' </remarks>
        Public Function profilLiklyhoodDimension() As Integer
            Dim k As Integer = -1

            Dim y1, y2 As Double
            Dim s1, s2 As Double
            Dim q1, pq1, p2 As Double
            Dim sigma, zaehler As Double
            Dim left, right As Double
            Dim tmpEV() As Double

            tmpEV = Me.EV

            If Me.MT < graphMatrix.matrixType.gausianWeightedMatrix Then
                For i As Integer = 0 To tmpEV.Count - 1
                    If tmpEV(i) <> 0 Then
                        tmpEV(i) = Math.Pow(tmpEV(i), -1)
                    End If
                Next
            End If

            Dim val As Double = Double.MinValue

            For tmpK As Integer = 0 To tmpEV.Count - 1
                y1 = avgEV(tmpEV, 0, tmpK, 1)
                y2 = avgEV(tmpEV, tmpK + 1, tmpEV.Count - 1, 1)

                s1 = avgEV(tmpEV, 0, tmpK, 2) - Math.Pow(y1, 2)
                s2 = avgEV(tmpEV, tmpK + 1, tmpEV.Count - 1, 2) - Math.Pow(y2, 2)

                q1 = tmpK
                pq1 = tmpEV.Count - (tmpK + 1) - 1
                p2 = tmpEV.Count - 2

                sigma = (q1 * s1 + s2 * pq1) / p2
                zaehler = 1 / Math.Pow((2 * Math.PI * sigma), 0.5)

                left = likelyhood(tmpEV, 0, tmpK, y1, sigma, zaehler)
                right = likelyhood(tmpEV, tmpK + 1, EV.Count - 1, y2, sigma, zaehler)

                If val > (left + right) Then
                    k = tmpK
                    Exit For
                End If
                val = left + right
            Next

            Return k
        End Function

        Private Function likelyhood(ByVal eV As Double(), _
    ByVal f As Integer, _
    ByVal t As Integer, _
    ByVal y As Double, _
    ByVal s As Double, _
    ByVal z As Double) As Double

            Dim sum As Double = 0
            Dim val As Double

            For i As Integer = f To t
                val = z * Math.Exp(-Math.Pow((eV(i) - y), 2) / (2 * s))
                sum += Math.Log(val, Math.E)
            Next

            Return sum
        End Function

        Private Function avgEV(ByVal eV As Double(), _
          ByVal f As Integer, _
          ByVal t As Integer, _
          ByVal pow As Integer) As Double

            Dim sum As Double = 0

            For i As Integer = f To t
                sum += Math.Pow(eV(i), pow)
            Next

            If sum <> 0 Then
                Return sum / (t - f + 1)
            Else
                Return 0
            End If
        End Function

        ' ---------------------- Shared ------------------------

    End Class
End Namespace
