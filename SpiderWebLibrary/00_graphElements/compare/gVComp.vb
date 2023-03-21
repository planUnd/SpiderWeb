Option Explicit On


Namespace graphElements
    Namespace compare


        ''' -------------------------------------------
        ''' Project : SpiderWebLibrary
        ''' Interface : gVComp
        ''' 
        ''' <summary>
        ''' Interface for all graphVertex compare elements.
        ''' </summary>
        ''' <remarks></remarks>
        ''' ''' <history>
        ''' [Richard Schaffranek]   22/11/2013 created
        ''' </history>
        ''' 

        Public Interface gVComp
            Inherits IComparer(Of graphVertex)
        End Interface
    End Namespace
End Namespace