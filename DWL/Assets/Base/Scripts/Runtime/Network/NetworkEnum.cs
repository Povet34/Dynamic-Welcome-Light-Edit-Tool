using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eRequestType
{
    None = -1,
    Get,
    Post,
    PostFile,
    Put,
    Delete,
}

public enum eMedicalRole
{
	None = -1,
	Patient,
	Doctor,
	Therapist,
}

public enum eGender
{
    None = -1,
    Male,
    Female,
}

/// <summary>
/// 에러 코드
/// </summary>
public enum Error
{
	/// <summary>
	/// 성공
	/// </summary>
	Success = 000,

	/// <summary>
	/// 세션 인증 실패 or 운영툴 강제 종료 (config 부터 재시작 처리)
	/// </summary>
	SessionError = 001,

	/// <summary>
	/// 세션 만료. (세션키 삭제됨)
	/// </summary>
	SessionExpired = 002,

	/// <summary>
	/// 중복 로그인 (세션키 변경)
	/// </summary>
	SessionChanged = 003,

	/// <summary>
	/// 토큰 만료됨 (자동로그인 풀림)
	/// </summary>
	TokenExpired = 004,

	/// <summary>
	/// 서버와 통신 실패 (클라 전용)
	/// </summary>
	ConnectionError = 005,

	/// <summary>
	/// 프로토콜 오류 (클라 전용)
	/// </summary>
	ProtocolError = 006,

	/// <summary>
	/// 데이터 오류 (클라 전용)
	/// </summary>
	DataProcessingError = 007,

	/// <summary>
	/// 의사 / 치료사 로그인 실패
	/// </summary>
	LoginFail = 008,

	/// <summary>
	/// 잘못된 파라미터 요청
	/// </summary>
	InvalidParameterRequest = 009,

	/// <summary>
	/// 병원 정보 없음
	/// </summary>
	NoHospitalInfomation = 010,

	/// <summary>
	/// 의사/치료사 정보 없음
	/// </summary>
	NoAccountInfomation = 013,

	/// <summary>
	/// 잘못된 치료사/의사 ID
	/// </summary>
	WRONG_ACCOUNT_ID = 014,

	/// <summary>
	/// 환자 정보 없음
	/// </summary>
	NoPatientInformation = 015,

	/// <summary>
	/// 검사 정보 없음
	/// </summary>
	NoInspectionInformation = 016,

	/// <summary>
	/// 파일 업로드 실패
	/// </summary>
	FileUploadFailed = 017,
}