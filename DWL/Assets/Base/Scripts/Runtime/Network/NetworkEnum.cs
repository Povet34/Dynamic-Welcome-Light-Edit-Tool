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
/// ���� �ڵ�
/// </summary>
public enum Error
{
	/// <summary>
	/// ����
	/// </summary>
	Success = 000,

	/// <summary>
	/// ���� ���� ���� or ��� ���� ���� (config ���� ����� ó��)
	/// </summary>
	SessionError = 001,

	/// <summary>
	/// ���� ����. (����Ű ������)
	/// </summary>
	SessionExpired = 002,

	/// <summary>
	/// �ߺ� �α��� (����Ű ����)
	/// </summary>
	SessionChanged = 003,

	/// <summary>
	/// ��ū ����� (�ڵ��α��� Ǯ��)
	/// </summary>
	TokenExpired = 004,

	/// <summary>
	/// ������ ��� ���� (Ŭ�� ����)
	/// </summary>
	ConnectionError = 005,

	/// <summary>
	/// �������� ���� (Ŭ�� ����)
	/// </summary>
	ProtocolError = 006,

	/// <summary>
	/// ������ ���� (Ŭ�� ����)
	/// </summary>
	DataProcessingError = 007,

	/// <summary>
	/// �ǻ� / ġ��� �α��� ����
	/// </summary>
	LoginFail = 008,

	/// <summary>
	/// �߸��� �Ķ���� ��û
	/// </summary>
	InvalidParameterRequest = 009,

	/// <summary>
	/// ���� ���� ����
	/// </summary>
	NoHospitalInfomation = 010,

	/// <summary>
	/// �ǻ�/ġ��� ���� ����
	/// </summary>
	NoAccountInfomation = 013,

	/// <summary>
	/// �߸��� ġ���/�ǻ� ID
	/// </summary>
	WRONG_ACCOUNT_ID = 014,

	/// <summary>
	/// ȯ�� ���� ����
	/// </summary>
	NoPatientInformation = 015,

	/// <summary>
	/// �˻� ���� ����
	/// </summary>
	NoInspectionInformation = 016,

	/// <summary>
	/// ���� ���ε� ����
	/// </summary>
	FileUploadFailed = 017,
}