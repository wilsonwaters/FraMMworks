/*M///////////////////////////////////////////////////////////////////////////////////////
//
//  IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING.
//
//  By downloading, copying, installing or using the software you agree to this license.
//  If you do not agree to this license, do not download, install,
//  copy or use the software.
//
//
//                        Intel License Agreement
//                For Open Source Computer Vision Library
//
// Copyright (C) 2000, Intel Corporation, all rights reserved.
// Third party copyrights are property of their respective owners.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//   * Redistribution's of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//   * Redistribution's in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
//   * The name of Intel Corporation may not be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall the Intel Corporation or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//
//M*/

// 2006-08-02  Roman Stanchak <rstancha@cse.wustl.edu>

%include "pycvseq.hpp"
%template (CvTuple_CvPoint_2) CvTuple<CvPoint,2>;
%template (CvTuple_float_2) CvTuple<float,2>;
%template (CvTuple_float_3) CvTuple<float,3>;

%template (CvSeq_CvPoint) CvTypedSeq<CvPoint>;
%template (CvSeq_CvPoint2D32f) CvTypedSeq<CvPoint2D32f>;
%template (CvSeq_CvRect) CvTypedSeq<CvRect>;
%template (CvSeq_CvSeq) CvTypedSeq<CvSeq *>;
%template (CvSeq_CvQuadEdge2D) CvTypedSeq<CvQuadEdge2D>;
%template (CvSeq_CvConnectedComp) CvTypedSeq<CvConnectedComp>;
%template (CvSeq_CvPoint_2) CvTypedSeq< CvTuple<CvPoint,2> >;
%template (CvSeq_float_2) CvTypedSeq< CvTuple<float,2> >;
%template (CvSeq_float_3) CvTypedSeq< CvTuple<float,3> >;

%extend CvSeq {
	%pythoncode %{
	def __iter__(self):
		"""
		generator function iterating elements in the sequence
		"""
		for i in range(self.total):
			yield self[i]

	def vrange(self):
		"""
		generator function iterating along v_next
		"""
		s = self
		t = type(self)
		while s:
			yield s
			s = t.cast(s.v_next)

	def hrange(self):
		"""
		generator function iterating along h_next
		"""
		s = self
		t = type(self)
		while s:
			yield s
			s = t.cast(s.h_next)
	%}
}

// accessor to turn edges into a typed sequence
%extend CvSubdiv2D {
	CvTypedSeq<CvQuadEdge2D> * typed_edges;
	CvTypedSeq<CvQuadEdge2D> * typed_edges_get(){
		return (CvTypedSeq<CvQuadEdge2D> *) self->edges;
	}
	void typed_edges_set( CvTypedSeq<CvQuadEdge2D> * ){
	}
	%pythoncode %{
	def __iter__(self):
		s = CvSeq_QuadEdge2D.cast(self)
		for i in range(s.total):
			yield s[i]
	%}
}

