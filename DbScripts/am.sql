CREATE SCHEMA IF NOT EXISTS "public";

--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.14
-- Dumped by pg_dump version 13.3

-- Started on 2021-11-04 20:39:43

--
-- TOC entry 2 (class 3079 OID 19689)
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;

CREATE EXTENSION postgis;


--
-- TOC entry 3163 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';

SET default_tablespace = '';


CREATE  TABLE "public".geo_municipalities ( 
	id                   uuid  NOT NULL  ,
	coordinates          geometry    ,
	name                 varchar  NOT NULL  ,
	date_off             timestamp with time zone,
	"level"              numeric DEFAULT 0 NOT NULL,
	CONSTRAINT pk_geo_municipalities_id PRIMARY KEY ( id )
 ) ;

COMMENT ON TABLE "public".geo_municipalities IS 'Геометрии муниципалитетов';

COMMENT ON COLUMN "public".geo_municipalities.coordinates IS 'Координаты полигона';

COMMENT ON COLUMN "public".geo_municipalities.name IS 'Наименование муниципалитета (административно-территориальной единицы)';

COMMENT ON COLUMN "public".geo_municipalities.date_off IS 'Дата удаления';

COMMENT ON COLUMN "public".geo_municipalities."level" IS 'Уровень вложенности муниципалитетов';


CREATE TABLE public.enterprise_data_access_level (
    id uuid NOT NULL,
    date_on timestamp without time zone,
    enterprise_id uuid NOT NULL,
    data_access_level_id uuid NOT NULL,
    date_off  timestamp with time zone

);

COMMENT ON TABLE "public".enterprise_data_access_level IS 'допуустимые для предприятия маркеры уровеней доступа по данным.\nСтруктура маркеров безопасности иерархическая';

COMMENT ON COLUMN "public".enterprise_data_access_level.date_on IS 'Дата активации записи';

COMMENT ON COLUMN "public".enterprise_data_access_level.enterprise_id IS 'ссылка на предприятие';

COMMENT ON COLUMN "public".enterprise_data_access_level.data_access_level_id IS 'ссылка на иерархический классификатор уровней доступа (Административные едницы)';

COMMENT ON COLUMN "public".enterprise_data_access_level.date_off IS 'Время логического удаления записи';


CREATE  TABLE "public".geo_municipalities_data_access_level ( 
	id                   uuid  NOT NULL ,
	geo_municipality_id  uuid  NOT NULL ,
	data_access_level_id uuid  NOT NULL ,
	date_off             timestamp   ,
	CONSTRAINT pk_geo_municipality_data_access_level_id PRIMARY KEY ( id )
 );


COMMENT ON TABLE "public".geo_municipalities_data_access_level IS 'Cвязь таблиц geo_municipalities и generic_types';

COMMENT ON COLUMN "public".geo_municipalities_data_access_level.geo_municipality_id IS 'ссылка на геометрии муниципалитетов';

COMMENT ON COLUMN "public".geo_municipalities_data_access_level.data_access_level_id IS 'ссылка на иерархический классификатор уровней доступа (Административные едницы)';

COMMENT ON TABLE "public".geo_municipalities_data_access_level IS 'Cвязь таблиц geo_municipalities и generic_types';

COMMENT ON COLUMN "public".geo_municipalities_data_access_level.geo_municipality_id IS 'ссылка на геометрии муниципалитетов';

COMMENT ON COLUMN "public".geo_municipalities_data_access_level.data_access_level_id IS 'ссылка на иерархический классификатор уровней доступа (Административные едницы)';

COMMENT ON COLUMN "public".geo_municipalities_data_access_level.date_off IS 'Дата удаления';



ALTER TABLE public.enterprise_data_access_level OWNER TO {{Database_User}};

--
-- TOC entry 3164 (class 0 OID 0)
-- Dependencies: 196
-- Name: TABLE enterprise_data_access_level; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.enterprise_data_access_level IS 'допуустимые для предприятия маркеры уровеней доступа по данным.\nСтруктура маркеров безопасности иерархическая';


--
-- TOC entry 3165 (class 0 OID 0)
-- Dependencies: 196
-- Name: COLUMN enterprise_data_access_level.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprise_data_access_level.date_on IS 'Дата активации записи';


--
-- TOC entry 3166 (class 0 OID 0)
-- Dependencies: 196
-- Name: COLUMN enterprise_data_access_level.enterprise_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprise_data_access_level.enterprise_id IS 'ссылка на предприятие';


--
-- TOC entry 3167 (class 0 OID 0)
-- Dependencies: 196
-- Name: COLUMN enterprise_data_access_level.data_access_level_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprise_data_access_level.data_access_level_id IS 'ссылка на иерархический классификатор уровней доступа (Административные едницы)';


--
-- TOC entry 3168 (class 0 OID 0)
-- Dependencies: 196
-- Name: COLUMN enterprise_data_access_level.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprise_data_access_level.date_off IS 'Время логического удаления записи';


--
-- TOC entry 186 (class 1259 OID 19700)
-- Name: enterprises; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.enterprises (
    id uuid NOT NULL,
    short_name character varying(100) NOT NULL,
    full_name character varying(140),
    address character varying(100),
    email character varying NOT NULL,
    phone character varying,
    is_integration boolean,
    web_site character varying,
    responsible_person_id uuid,
    date_on timestamp without time zone,
    date_off timestamp without time zone,
    inn character varying,
    kpp character varying,
    ogrn character varying,
    account character varying,
    bank character varying,
    cor_account character varying,
    bik character varying
);


ALTER TABLE public.enterprises OWNER TO {{Database_User}};

--
-- TOC entry 3169 (class 0 OID 0)
-- Dependencies: 186
-- Name: TABLE enterprises; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.enterprises IS 'Список организаций';


--
-- TOC entry 3170 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.short_name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.short_name IS 'Краткое название организации';


--
-- TOC entry 3171 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.full_name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.full_name IS 'Полное имя организации';


--
-- TOC entry 3172 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.address; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.address IS 'Адрес';


--
-- TOC entry 3173 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.email; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.email IS 'Почтовый адрес';


--
-- TOC entry 3174 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.phone; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.phone IS 'Телефон';


--
-- TOC entry 3175 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.is_integration; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.is_integration IS 'Указанная организация задействована в интеграции, удаление невозможно';


--
-- TOC entry 3176 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.web_site; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.web_site IS 'Веб сайт';


--
-- TOC entry 3177 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.responsible_person_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.responsible_person_id IS 'Ответственный за организацию';


--
-- TOC entry 3178 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.date_on IS 'Дата активации записи';


--
-- TOC entry 3179 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3180 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.inn; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.inn IS 'ИНН';


--
-- TOC entry 3181 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.kpp; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.kpp IS 'КПП';


--
-- TOC entry 3182 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.ogrn; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.ogrn IS 'ОГРН';


--
-- TOC entry 3183 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.account; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.account IS 'Расчетный счет';


--
-- TOC entry 3184 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.bank; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.bank IS 'Банк';


--
-- TOC entry 3185 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.cor_account; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.cor_account IS 'Корреспондентский счет';


--
-- TOC entry 3186 (class 0 OID 0)
-- Dependencies: 186
-- Name: COLUMN enterprises.bik; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.enterprises.bik IS 'БИК';


--
-- TOC entry 199 (class 1259 OID 268948)
-- Name: external_objects; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.external_objects (
    id uuid NOT NULL,
    date_on timestamp with time zone NOT NULL,
    object_id uuid NOT NULL,
    system_id uuid NOT NULL,
    external_id character varying(255) NOT NULL,
    hash character varying(64),
    date_off timestamp with time zone
);


ALTER TABLE public.external_objects OWNER TO {{Database_User}};

--
-- TOC entry 3187 (class 0 OID 0)
-- Dependencies: 199
-- Name: TABLE external_objects; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.external_objects IS 'Мапинг объектов во внешние системы';


--
-- TOC entry 3188 (class 0 OID 0)
-- Dependencies: 199
-- Name: COLUMN external_objects.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.external_objects.date_on IS 'Дата-время первоначального создания';


--
-- TOC entry 3189 (class 0 OID 0)
-- Dependencies: 199
-- Name: COLUMN external_objects.object_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.external_objects.object_id IS 'Идентификатор объекта нашей системы(ид пользователя или предприятия)';


--
-- TOC entry 3190 (class 0 OID 0)
-- Dependencies: 199
-- Name: COLUMN external_objects.system_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.external_objects.system_id IS 'Идентификатор внешней системы';


--
-- TOC entry 3191 (class 0 OID 0)
-- Dependencies: 199
-- Name: COLUMN external_objects.hash; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.external_objects.hash IS 'Значение хэш функции объекта, полученное при синхронизации объектов, импользовать SHA256';


--
-- TOC entry 3192 (class 0 OID 0)
-- Dependencies: 199
-- Name: COLUMN external_objects.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.external_objects.date_off IS 'Дата-время логического удаления объекта';


--
-- TOC entry 187 (class 1259 OID 19706)
-- Name: generic_types; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.generic_types (
    name character varying NOT NULL,
    short_name character varying,
    code character varying,
    parent_id uuid,
    id uuid NOT NULL,
    date_off  timestamp with time zone
);


ALTER TABLE public.generic_types OWNER TO {{Database_User}};

--
-- TOC entry 3193 (class 0 OID 0)
-- Dependencies: 187
-- Name: TABLE generic_types; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.generic_types IS 'Типы';


--
-- TOC entry 3194 (class 0 OID 0)
-- Dependencies: 187
-- Name: COLUMN generic_types.name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.generic_types.name IS 'Полное имя';


--
-- TOC entry 3195 (class 0 OID 0)
-- Dependencies: 187
-- Name: COLUMN generic_types.short_name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.generic_types.short_name IS 'Краткое имя';


--
-- TOC entry 3196 (class 0 OID 0)
-- Dependencies: 187
-- Name: COLUMN generic_types.code; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.generic_types.code IS 'Код';


--
-- TOC entry 3197 (class 0 OID 0)
-- Dependencies: 187
-- Name: COLUMN generic_types.parent_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.generic_types.parent_id IS 'Родительский тип';


--
-- TOC entry 3198 (class 0 OID 0)
-- Dependencies: 187
-- Name: COLUMN generic_types.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.generic_types.date_off IS 'Время логического удаления записи';


--
-- TOC entry 188 (class 1259 OID 19712)
-- Name: persons; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.persons (
    id uuid NOT NULL,
    first_name character varying(100) NOT NULL,
    last_name character varying(100) NOT NULL,
    middle_name character varying(100),
    "position" character varying,
    enterprise_id uuid NOT NULL,
    phone character varying,
    email character varying,
    date_off timestamp without time zone,
    date_on timestamp without time zone
);


ALTER TABLE public.persons OWNER TO {{Database_User}};

--
-- TOC entry 3199 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.first_name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.first_name IS 'Имя';


--
-- TOC entry 3200 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.last_name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.last_name IS 'Фамилия';


--
-- TOC entry 3201 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.middle_name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.middle_name IS 'Отчество';


--
-- TOC entry 3202 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons."position"; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons."position" IS 'Должность (в реальном мире)';


--
-- TOC entry 3203 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.enterprise_id; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.enterprise_id IS 'Ссылка на организацию';


--
-- TOC entry 3204 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.phone; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.phone IS 'Телефон';


--
-- TOC entry 3205 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.email; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.email IS 'Почтовый адрес';


--
-- TOC entry 3206 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3207 (class 0 OID 0)
-- Dependencies: 188
-- Name: COLUMN persons.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.persons.date_on IS 'Дата активации записи';


--
-- TOC entry 189 (class 1259 OID 19718)
-- Name: right_group_rights; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.right_group_rights (
    right_id uuid NOT NULL,
    right_group_id uuid NOT NULL
);


ALTER TABLE public.right_group_rights OWNER TO {{Database_User}};

--
-- TOC entry 190 (class 1259 OID 19721)
-- Name: right_groups; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.right_groups (
    id uuid NOT NULL,
    "order" integer NOT NULL,
    name character varying(100) NOT NULL,
    date_off timestamp without time zone,
    date_on timestamp without time zone
);


ALTER TABLE public.right_groups OWNER TO {{Database_User}};

--
-- TOC entry 3208 (class 0 OID 0)
-- Dependencies: 190
-- Name: TABLE right_groups; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.right_groups IS 'Список групп прав';


--
-- TOC entry 3209 (class 0 OID 0)
-- Dependencies: 190
-- Name: COLUMN right_groups."order"; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.right_groups."order" IS 'Порядковый номер';


--
-- TOC entry 3210 (class 0 OID 0)
-- Dependencies: 190
-- Name: COLUMN right_groups.name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.right_groups.name IS 'Название группы';


--
-- TOC entry 3211 (class 0 OID 0)
-- Dependencies: 190
-- Name: COLUMN right_groups.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.right_groups.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3212 (class 0 OID 0)
-- Dependencies: 190
-- Name: COLUMN right_groups.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.right_groups.date_on IS 'Дата активации записи';


--
-- TOC entry 191 (class 1259 OID 19724)
-- Name: rights; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.rights (
    id uuid NOT NULL,
    name character varying(100),
    alias character varying,
    date_off timestamp without time zone,
    date_on timestamp without time zone,
    owner character varying,
    "order" integer NOT NULL
);


ALTER TABLE public.rights OWNER TO {{Database_User}};

--
-- TOC entry 3213 (class 0 OID 0)
-- Dependencies: 191
-- Name: TABLE rights; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.rights IS 'Список прав';


--
-- TOC entry 3214 (class 0 OID 0)
-- Dependencies: 191
-- Name: COLUMN rights.name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.rights.name IS 'Название права';


--
-- TOC entry 3215 (class 0 OID 0)
-- Dependencies: 191
-- Name: COLUMN rights.alias; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.rights.alias IS 'Псевдоним для фронта';


--
-- TOC entry 3216 (class 0 OID 0)
-- Dependencies: 191
-- Name: COLUMN rights.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.rights.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3217 (class 0 OID 0)
-- Dependencies: 191
-- Name: COLUMN rights.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.rights.date_on IS 'Дата активации записи';


--
-- TOC entry 3218 (class 0 OID 0)
-- Dependencies: 191
-- Name: COLUMN rights.owner; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.rights.owner IS 'ссылка на проект, где, реализована фича, возможно ссылка эквивалентна названию конфигурации';


--
-- TOC entry 3219 (class 0 OID 0)
-- Dependencies: 191
-- Name: COLUMN rights."order"; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.rights."order" IS 'Порядковый номер';


--
-- TOC entry 192 (class 1259 OID 19730)
-- Name: role_rights; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.role_rights (
    role_id uuid NOT NULL,
    right_id uuid NOT NULL,
    date_off timestamp without time zone,
    date_on timestamp without time zone
);


ALTER TABLE public.role_rights OWNER TO {{Database_User}};

--
-- TOC entry 3220 (class 0 OID 0)
-- Dependencies: 192
-- Name: COLUMN role_rights.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.role_rights.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3221 (class 0 OID 0)
-- Dependencies: 192
-- Name: COLUMN role_rights.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.role_rights.date_on IS 'Дата активации записи';


--
-- TOC entry 193 (class 1259 OID 19733)
-- Name: roles; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.roles (
    id uuid NOT NULL,
    name character varying,
    date_off timestamp without time zone,
    date_on timestamp without time zone
);


ALTER TABLE public.roles OWNER TO {{Database_User}};

--
-- TOC entry 3222 (class 0 OID 0)
-- Dependencies: 193
-- Name: COLUMN roles.name; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.roles.name IS 'Название роли';


--
-- TOC entry 3223 (class 0 OID 0)
-- Dependencies: 193
-- Name: COLUMN roles.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.roles.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3224 (class 0 OID 0)
-- Dependencies: 193
-- Name: COLUMN roles.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.roles.date_on IS 'Дата активации записи';


--
-- TOC entry 194 (class 1259 OID 19739)
-- Name: user_roles; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.user_roles (
    role_id uuid NOT NULL,
    user_id uuid NOT NULL,
    date_off timestamp without time zone,
    date_on timestamp without time zone
);


ALTER TABLE public.user_roles OWNER TO {{Database_User}};

--
-- TOC entry 3225 (class 0 OID 0)
-- Dependencies: 194
-- Name: TABLE user_roles; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON TABLE public.user_roles IS 'Дата активации записи';


--
-- TOC entry 3226 (class 0 OID 0)
-- Dependencies: 194
-- Name: COLUMN user_roles.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.user_roles.date_off IS 'Дата логического удаления записи = дата истечения прав';


--
-- TOC entry 3227 (class 0 OID 0)
-- Dependencies: 194
-- Name: COLUMN user_roles.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.user_roles.date_on IS 'Дата активации записи';


--
-- TOC entry 195 (class 1259 OID 19742)
-- Name: users; Type: TABLE; Schema: public; Owner: {{Database_User}}
--

CREATE TABLE public.users (
    id uuid NOT NULL,
    login character varying NOT NULL,
    person_id uuid,
    password character varying NOT NULL,
    date_off timestamp without time zone,
    date_on timestamp without time zone,
    token character varying,
    queue character varying,
    is_blocked boolean NOT NULL,
    login_fail_count integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.users OWNER TO {{Database_User}};

--
-- TOC entry 3228 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.login; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.login IS 'Логин';


--
-- TOC entry 3229 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.password; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.password IS 'Хэш пароля';


--
-- TOC entry 3230 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.date_off; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.date_off IS 'Дата логического удаления записи';


--
-- TOC entry 3231 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.date_on; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.date_on IS 'Дата активации записи';


--
-- TOC entry 3232 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.token; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.token IS 'Refresh Токен (долгий)';


--
-- TOC entry 3233 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.queue; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.queue IS 'имя очереди';


--
-- TOC entry 3234 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.is_blocked; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.is_blocked IS 'признак блокировки пользователя';


--
-- TOC entry 3235 (class 0 OID 0)
-- Dependencies: 195
-- Name: COLUMN users.login_fail_count; Type: COMMENT; Schema: public; Owner: {{Database_User}}
--

COMMENT ON COLUMN public.users.login_fail_count IS 'кол-во неудачных попыток входа';


--
-- TOC entry 3023 (class 2606 OID 41724)
-- Name: enterprise_data_access_level pk_enterprise_data_acceess_level_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.enterprise_data_access_level
    ADD CONSTRAINT pk_enterprise_data_acceess_level_id PRIMARY KEY (id);


--
-- TOC entry 3027 (class 2606 OID 268952)
-- Name: external_objects pk_external_objects_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.external_objects
    ADD CONSTRAINT pk_external_objects_id PRIMARY KEY (id);


--
-- TOC entry 2997 (class 2606 OID 19749)
-- Name: generic_types pk_generic_types_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.generic_types
    ADD CONSTRAINT pk_generic_types_id PRIMARY KEY (id);


--
-- TOC entry 2994 (class 2606 OID 19751)
-- Name: enterprises pk_organizations_id ; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.enterprises
    ADD CONSTRAINT "pk_organizations_id " PRIMARY KEY (id);


--
-- TOC entry 3000 (class 2606 OID 19753)
-- Name: persons pk_persons_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.persons
    ADD CONSTRAINT pk_persons_id PRIMARY KEY (id);


--
-- TOC entry 3006 (class 2606 OID 19755)
-- Name: right_groups pk_right_groups_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.right_groups
    ADD CONSTRAINT pk_right_groups_id PRIMARY KEY (id);


--
-- TOC entry 3008 (class 2606 OID 19757)
-- Name: rights pk_rights_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.rights
    ADD CONSTRAINT pk_rights_id PRIMARY KEY (id);


--
-- TOC entry 3014 (class 2606 OID 19759)
-- Name: roles pk_roles_id_0; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT pk_roles_id_0 PRIMARY KEY (id);


--
-- TOC entry 3021 (class 2606 OID 19761)
-- Name: users pk_users_id; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT pk_users_id PRIMARY KEY (id);


--
-- TOC entry 3004 (class 2606 OID 19763)
-- Name: right_group_rights right_group_right_primary_key; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.right_group_rights
    ADD CONSTRAINT right_group_right_primary_key PRIMARY KEY (right_id, right_group_id);


--
-- TOC entry 3012 (class 2606 OID 19765)
-- Name: role_rights role_right_primaky_key; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.role_rights
    ADD CONSTRAINT role_right_primaky_key PRIMARY KEY (role_id, right_id);


--
-- TOC entry 3018 (class 2606 OID 19767)
-- Name: user_roles user_role_primary_key; Type: CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.user_roles
    ADD CONSTRAINT user_role_primary_key PRIMARY KEY (user_id, role_id);


--
-- TOC entry 2992 (class 1259 OID 19768)
-- Name: idx_enterprise_responsible_user_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_enterprise_responsible_user_id ON public.enterprises USING btree (responsible_person_id);


--
-- TOC entry 2995 (class 1259 OID 19769)
-- Name: idx_generic_types_parent_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_generic_types_parent_id ON public.generic_types USING btree (parent_id);


--
-- TOC entry 2998 (class 1259 OID 19770)
-- Name: idx_persons_organization_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_persons_organization_id ON public.persons USING btree (enterprise_id);


--
-- TOC entry 3001 (class 1259 OID 19771)
-- Name: idx_right_group_right_right_group_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_right_group_right_right_group_id ON public.right_group_rights USING btree (right_group_id);


--
-- TOC entry 3002 (class 1259 OID 19772)
-- Name: idx_right_group_right_right_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_right_group_right_right_id ON public.right_group_rights USING btree (right_id);


--
-- TOC entry 3009 (class 1259 OID 19773)
-- Name: idx_role_rights_right_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_role_rights_right_id ON public.role_rights USING btree (right_id);


--
-- TOC entry 3010 (class 1259 OID 19774)
-- Name: idx_role_rights_role_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_role_rights_role_id ON public.role_rights USING btree (role_id);


--
-- TOC entry 3015 (class 1259 OID 19775)
-- Name: idx_user_roles_role_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_user_roles_role_id ON public.user_roles USING btree (role_id);


--
-- TOC entry 3016 (class 1259 OID 19776)
-- Name: idx_user_roles_user_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_user_roles_user_id ON public.user_roles USING btree (user_id);


--
-- TOC entry 3019 (class 1259 OID 19777)
-- Name: idx_users_person_id; Type: INDEX; Schema: public; Owner: {{Database_User}}
--

CREATE INDEX idx_users_person_id ON public.users USING btree (person_id);


--
-- TOC entry 3039 (class 2606 OID 41730)
-- Name: enterprise_data_access_level fk_enterprise_data_acceess_level_enterprises; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.enterprise_data_access_level
    ADD CONSTRAINT fk_enterprise_data_acceess_level_enterprises FOREIGN KEY (enterprise_id) REFERENCES public.enterprises(id);


--
-- TOC entry 3038 (class 2606 OID 41725)
-- Name: enterprise_data_access_level fk_enterprise_data_access_level_generic_types; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.enterprise_data_access_level
    ADD CONSTRAINT fk_enterprise_data_acceess_level_generic_types FOREIGN KEY (data_access_level_id) REFERENCES public.generic_types(id);


--
-- TOC entry 3028 (class 2606 OID 19778)
-- Name: enterprises fk_enterprises_persons; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.enterprises
    ADD CONSTRAINT fk_enterprises_persons FOREIGN KEY (responsible_person_id) REFERENCES public.persons(id);


--
-- TOC entry 3040 (class 2606 OID 268953)
-- Name: external_objects fk_external_objects_generic_types; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.external_objects
    ADD CONSTRAINT fk_external_objects_generic_types FOREIGN KEY (system_id) REFERENCES public.generic_types(id);


--
-- TOC entry 3029 (class 2606 OID 19783)
-- Name: generic_types fk_generic_types_generic_types; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.generic_types
    ADD CONSTRAINT fk_generic_types_generic_types FOREIGN KEY (parent_id) REFERENCES public.generic_types(id);


--
-- TOC entry 3030 (class 2606 OID 19788)
-- Name: persons fk_persons_enterprise; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.persons
    ADD CONSTRAINT fk_persons_enterprise FOREIGN KEY (enterprise_id) REFERENCES public.enterprises(id);


--
-- TOC entry 3031 (class 2606 OID 19793)
-- Name: right_group_rights fk_right_group_right_group_rights; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.right_group_rights
    ADD CONSTRAINT fk_right_group_right_group_rights FOREIGN KEY (right_group_id) REFERENCES public.right_groups(id);


--
-- TOC entry 3032 (class 2606 OID 19798)
-- Name: right_group_rights fk_right_right_group_rights; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.right_group_rights
    ADD CONSTRAINT fk_right_right_group_rights FOREIGN KEY (right_id) REFERENCES public.rights(id);


--
-- TOC entry 3033 (class 2606 OID 19803)
-- Name: role_rights fk_role_rights_rights; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.role_rights
    ADD CONSTRAINT fk_role_rights_rights FOREIGN KEY (right_id) REFERENCES public.rights(id);


--
-- TOC entry 3034 (class 2606 OID 19808)
-- Name: role_rights fk_role_rights_roles; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.role_rights
    ADD CONSTRAINT fk_role_rights_roles FOREIGN KEY (role_id) REFERENCES public.roles(id);


--
-- TOC entry 3035 (class 2606 OID 19813)
-- Name: user_roles fk_user_roles_roles; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.user_roles
    ADD CONSTRAINT fk_user_roles_roles FOREIGN KEY (role_id) REFERENCES public.roles(id);


--
-- TOC entry 3036 (class 2606 OID 19818)
-- Name: user_roles fk_user_roles_users; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.user_roles
    ADD CONSTRAINT fk_user_roles_users FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- TOC entry 3037 (class 2606 OID 19823)
-- Name: users fk_users_persons; Type: FK CONSTRAINT; Schema: public; Owner: {{Database_User}}
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT fk_users_persons FOREIGN KEY (person_id) REFERENCES public.persons(id);



ALTER TABLE "public".geo_municipalities_data_access_level ADD CONSTRAINT fk_data_access_level_id FOREIGN KEY ( data_access_level_id ) REFERENCES "public".generic_types( id )   ;

ALTER TABLE "public".geo_municipalities_data_access_level ADD CONSTRAINT fk_geo_municipality_id FOREIGN KEY ( geo_municipality_id ) REFERENCES "public".geo_municipalities( id )   ;

-- TABLE public.object_changes;

CREATE TABLE "public".object_changes (
	object_id uuid NOT NULL,
	alias varchar(50) NOT NULL,
	is_changed bool NULL,
	date_changed timestamptz NOT NULL DEFAULT now(),
	hash varchar(64) NOT NULL,
	date_off timestamptz NULL
	);


CREATE INDEX idx_object_changes_date_changed ON "public".object_changes USING btree (date_changed);

COMMENT ON TABLE "public".object_changes IS 'Таблица изменений объекта - хэша объекта';
COMMENT ON COLUMN "public".object_changes.object_id IS 'Идентификатор объекта (пользователь, логин и тд)';
COMMENT ON COLUMN "public".object_changes.alias IS 'Алиас объекта User и тд';
COMMENT ON COLUMN "public".object_changes.is_changed IS 'Признак изменения объекта ( если null объект новый) ';
COMMENT ON COLUMN "public".object_changes.date_changed IS 'Время изменения ';
COMMENT ON COLUMN "public".object_changes.hash IS 'Хэш объекта  ';
COMMENT ON COLUMN "public".object_changes.date_off IS 'Признак удаления объекта ';

-- Completed on 2021-11-04 20:39:51

--
-- PostgreSQL database dump complete
--

