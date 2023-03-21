Option Explicit On


Namespace graphElements
    Namespace compare

        ''' -------------------------------------------
        ''' Project : SpiderWebLibrary
        ''' Interface : gEComp
        ''' 
        ''' <summary>
        ''' Interface for all graphEdge compare elements.
        ''' </summary>
        ''' <remarks></remarks>
        ''' ''' <history>
        ''' [Richard Schaffranek]   22/11/2013 created
        ''' </history>
        ''' 
        Public Interface gEComp
            Inherits IComparer(Of graphEdge)
        End Interface
    End Namespace
End Namespace